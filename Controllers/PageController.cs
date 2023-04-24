using Microsoft.AspNetCore.Mvc;
using Penguin.Cms.Pages;
using Penguin.Cms.Pages.Repositories;
using Penguin.Cms.Web.Pages.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using TemplateParameter = Penguin.Templating.Abstractions.TemplateParameter;

namespace Penguin.Cms.Modules.Pages.Controllers
{
    public class PageController : Controller
    {
        protected PageRenderer PageRenderer { get; set; }

        protected PageRepository PageRepository { get; set; }

        public PageController(PageRepository pageRepository, PageRenderer pageRenderer)
        {
            PageRenderer = pageRenderer;
            PageRepository = pageRepository;
        }

        public ActionResult RenderPage(string Url)
        {
            Url ??= RouteData?.Values["Url"]?.ToString() ?? throw new ArgumentNullException(nameof(Url));

            //ToDo move this back to a cache
            Page page = PageRepository.GetByUrl(Url);
            string content = page.Content;

            if (Page.GetPageType(Url) == Page.PageType.CSS)
            {
                return Content(content, "text/css");
            }
            else if (Page.GetPageType(Url) == Page.PageType.JS)
            {
                return Content(content, "application/javascript");
            }
            else
            {
                List<TemplateParameter> templateParameters = new();

                foreach (string k in Request.Query.Keys)
                {
                    if (page.Parameters.FirstOrDefault(t => t.Name == k) is Penguin.Cms.Pages.TemplateParameter t)
                    {
                        templateParameters.Add(t);
                    }
                }

                (string RelativePath, object Model) = PageRenderer.GenerateRenderInformation(page, templateParameters);
                return View(RelativePath.Replace('\\', '/'), Model);
            }
        }

    
    }
}