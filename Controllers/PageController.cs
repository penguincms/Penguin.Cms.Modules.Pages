using Microsoft.AspNetCore.Mvc;
using Penguin.Cms.Modules.Pages.Rendering;
using Penguin.Cms.Pages;
using Penguin.Cms.Pages.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TemplateParameter = Penguin.Templating.Abstractions.TemplateParameter;

namespace Penguin.Cms.Modules.Pages.Controllers
{
    [SuppressMessage("Design", "CA1054:Uri parameters should not be strings")]
    public class PageController : Controller
    {
        protected PageRenderer PageRenderer { get; set; }

        protected PageRepository PageRepository { get; set; }

        public PageController(PageRepository pageRepository, PageRenderer pageRenderer)
        {
            this.PageRenderer = pageRenderer;
            PageRepository = pageRepository;
        }

        public ActionResult RenderPage(string Url)
        {
            Url ??= this.RouteData?.Values["Url"]?.ToString() ?? throw new ArgumentNullException(nameof(Url));

            //ToDo move this back to a cache
            Page page = this.PageRepository.GetByUrl(Url);
            string content = page.Content;

            if (Page.GetPageType(Url) == Page.PageType.CSS)
            {
                return this.Content(content, "text/css");
            }
            else if (Page.GetPageType(Url) == Page.PageType.JS)
            {
                return this.Content(content, "application/javascript");
            }
            else
            {
                List<TemplateParameter> templateParameters = new List<TemplateParameter>();

                foreach (string k in this.Request.Query.Keys)
                {
                    if (page.Parameters.FirstOrDefault(t => t.Name == k) is Penguin.Cms.Pages.TemplateParameter t)
                    {
                        templateParameters.Add(t);
                    }
                }

                (string RelativePath, object Model) = this.PageRenderer.GenerateRenderInformation(page, templateParameters);
                return this.View(RelativePath.Replace('\\', '/'), Model);
            }
        }
    }
}