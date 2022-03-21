using Microsoft.AspNetCore.Html;
using Penguin.Cms.Abstractions;
using Penguin.Cms.Abstractions.Interfaces;
using Penguin.Cms.Modules.Pages.Rendering;
using Penguin.Cms.Pages;
using Penguin.Cms.Pages.Repositories;
using Penguin.Messaging.Abstractions.Interfaces;
using Penguin.Messaging.Persistence.Messages;
using Penguin.Web.Mvc.Abstractions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Penguin.Cms.Modules.Pages.Macros
{
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    public class Template : IMessageHandler<Updated<Page>>, IMessageHandler<Created<Page>>, IMessageHandler<Penguin.Messaging.Application.Messages.Startup>, IMacroProvider
    {
        private static readonly List<Macro> TemplateMacros = new List<Macro>();
        protected PageRenderer PageRenderer { get; set; }

        protected PageRepository PageRepository { get; set; }

        protected IViewRenderService ViewRenderService { get; set; }

        public Template(PageRenderer pageRenderer, IViewRenderService viewRenderService, PageRepository pageRepository)
        {
            this.PageRenderer = pageRenderer;
            this.ViewRenderService = viewRenderService;
            this.PageRepository = pageRepository;
        }

        public void AcceptMessage(Updated<Page> page) => this.Refresh();

        public void AcceptMessage(Created<Page> page) => this.Refresh();

        public void AcceptMessage(Penguin.Messaging.Application.Messages.Startup startup) => this.Refresh();

        public List<Macro> GetMacros(object o) => TemplateMacros;

        public HtmlString Render(string Url, object? Model = null)
        {
            Page page = this.PageRepository.GetByUrl(Url);

            List<Penguin.Templating.Abstractions.TemplateParameter> parameters = new List<Penguin.Templating.Abstractions.TemplateParameter>();

            if (Model != null)
            {
                foreach (PropertyInfo p in Model.GetType().GetProperties())
                {
                    parameters.Add(new Penguin.Templating.Abstractions.TemplateParameter()
                    {
                        Name = p.Name,
                        Type = p.PropertyType,
                        Value = p.GetValue(Model)
                    });
                }
            }

            (string RelativePath, object templateModel) = this.PageRenderer.GenerateRenderInformation(page, parameters);

            Task<string> result = this.ViewRenderService.RenderToStringAsync(RelativePath, "", templateModel, true);

            result.Wait();

            return new HtmlString(result.Result);
        }

        private static Macro PageToMacro(Page thisPage)
        {
            string ParameterString = string.Empty;

            if (thisPage.Parameters.Any())
            {
                ParameterString = $", new {{{string.Join(", ", thisPage.Parameters.Select(p => $"{p.Name} = \"\""))}}}";
            }

            Macro macro = new Macro
            ("Template",
                 $"@Template.Render(\"{thisPage.Url}\"{ParameterString})"

            );

            return macro;
        }

        private void Refresh()
        {
            lock (TemplateMacros)
            {
                TemplateMacros.Clear();

                IEnumerable<Page> allPages = this.PageRepository.Where(p => p.Type == Page.PageType.HTML || p.Type == Page.PageType.WYSIWYG);

                foreach (Page thisPage in allPages)
                {
                    TemplateMacros.Add(PageToMacro(thisPage));
                }
            }
        }
    }
}