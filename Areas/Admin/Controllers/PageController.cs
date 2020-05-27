using Microsoft.AspNetCore.Mvc;
using Penguin.Cms.Core.Services;
using Penguin.Cms.Entities;
using Penguin.Cms.Modules.Dynamic.Areas.Admin.Controllers;
using Penguin.Cms.Modules.Pages.Areas.Admin.Models;
using Penguin.Cms.Modules.Pages.Constants.Strings;
using Penguin.Cms.Pages;
using Penguin.Cms.Pages.Repositories;
using Penguin.Cms.Web.Modules;
using Penguin.Persistence.Abstractions.Interfaces;
using Penguin.Security.Abstractions.Interfaces;
using Penguin.Web.Security.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Penguin.Cms.Modules.Pages.Areas.Admin.Controllers
{
    //I dont remember why theres so much manual binding in this class. Should probably be fixed, if there isn't a reason
    [RequiresRole(RoleNames.CONTENT_MANAGER)]
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
    [SuppressMessage("Design", "CA1054:Uri parameters should not be strings")]
    public class PageController : ObjectManagementController<Page>
    {
        private const string NULL_BASE_URL_MESSAGE = "Base Url can not be null";
        private const string NULL_PAGE_MESSAGE = "Page on model can not be null";
        private const string NULL_PAGE_URL_MESSAGE = "Page Url can not be null";
        protected PageRepository PageRepository { get; set; }

        public PageController(ComponentService componentService, PageRepository pageRepository, IServiceProvider serviceProvider, IUserSession userSession) : base(serviceProvider, userSession)
        {
            this.ComponentService = componentService;
            this.PageRepository = pageRepository;
        }

        public ActionResult AddCSS(string Url)
        {
            EditNodePageModel model = new EditNodePageModel(Url, this.PageRepository.GetByUrl(Url));

            return this.View(model);
        }

        public ActionResult AddFolder(string Url)
        {
            EditNodePageModel model = new EditNodePageModel(Url);

            return this.View(model);
        }

        [HttpPost]
        public ActionResult AddFolder(EditNodePageModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.BaseUrl is null)
            {
                throw new NullReferenceException(NULL_BASE_URL_MESSAGE);
            }

            if (model.Page?.Url is null)
            {
                throw new NullReferenceException(NULL_PAGE_URL_MESSAGE);
            }

            using (IWriteContext writeContext = this.PageRepository.WriteContext())
            {
                Page thisPage = new Page
                {
                    Url = Path.Combine(model.BaseUrl, model.Page.Url).Replace('\\', '/')
                };

                this.PageRepository.AddOrUpdate(thisPage);
            }

            return this.Redirect("/Admin/Page/Index");
        }

        public ActionResult AddJS(string Url)
        {
            EditNodePageModel model = new EditNodePageModel(Url, this.PageRepository.GetByUrl(Url));

            return this.View(model);
        }

        public ActionResult AddPage(string Url)
        {
            EditNodePageModel model = new EditNodePageModel(Url)
            {
                Macros = new MacroService(this.ServiceProvider).GetMacros(null)
            };

            return this.View("EditNode", model);
        }

        [HttpPost]
        public ActionResult AddPage(EditNodePageModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.BaseUrl is null)
            {
                throw new NullReferenceException(NULL_BASE_URL_MESSAGE);
            }

            if (model.Page?.Url is null)
            {
                throw new NullReferenceException(NULL_PAGE_URL_MESSAGE);
            }

            using (IWriteContext writeContext = this.PageRepository.WriteContext())
            {
                model.Page.Url = Path.Combine(model.BaseUrl, model.Page.Url ?? "").Replace("\\", "/", StringComparison.OrdinalIgnoreCase);

                this.PageRepository.AddOrUpdate(model.Page);
            }

            return this.Redirect("/Admin/Page/Index");
        }

        public ActionResult DeleteNode(string Url)
        {
            using (IWriteContext writeContext = this.PageRepository.WriteContext())
            {
                Page thisPage = this.PageRepository.GetByUrl(Url);

                thisPage.DateDeleted = DateTime.Now;
            }

            return this.Redirect("/Admin/Page/Index");
        }

        public ActionResult EditNode(string Url)
        {
            EditNodePageModel model = new EditNodePageModel(Url, this.PageRepository.GetByUrl(Url));

            if (model.Page is null)
            {
                throw new NullReferenceException(NULL_PAGE_MESSAGE);
            }

            model.Modules = this.ComponentService.GetComponents<ViewModule, Entity>(model.Page).ToList();

            if (Page.GetPageType(Url) == Page.PageType.CSS)
            {
                return this.View("EditCSS", model);
            }
            else if (Page.GetPageType(Url) == Page.PageType.JS)
            {
                return this.View("EditJS", model);
            }

            model.Macros = new MacroService(this.ServiceProvider).GetMacros(model.Page);

            return this.View(model);
        }

        [HttpPost]
        public ActionResult EditNode(EditNodePageModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Page is null)
            {
                throw new NullReferenceException(NULL_PAGE_MESSAGE);
            }

            using (IWriteContext writeContext = this.PageRepository.WriteContext())
            {
                Page thisPage = this.PageRepository.Find(model.Page._Id);

                thisPage.Content = model.Page.Content;

                thisPage.Type = model.Page.Type;

                thisPage.Url = model.Page.Url;

                thisPage.Cascade = model.Page.Cascade;

                thisPage.Layout = model.Page.Layout;

                thisPage.Parameters = model.Page.Parameters;

                this.PageRepository.AddOrUpdate(thisPage);
            }

            return this.Redirect("/Admin/Page/Index");
        }

        private PageTreeDirectory BuildPageDirectory(List<Page> Pages, PageTreeDirectory root)
        {
            string CurrentDir = root.Url;
            string MatchUrl = CurrentDir.EndsWith("/", StringComparison.Ordinal) ? CurrentDir : CurrentDir + "/";

            List<Page> Children = Pages.Where(p =>
                                        string.Equals(
                                            new FileInfo(p.Url).Directory.FullName,
                                            new DirectoryInfo(CurrentDir).FullName,
                                            StringComparison.InvariantCultureIgnoreCase))
                                       .ToList();

            foreach (Page child in Children)
            {
                PageTreeDirectory model = new PageTreeDirectory(child);

                Pages.Remove(model.Page);

                model = this.BuildPageDirectory(Pages, model);

                root.Children.Add(model);
            }

            return root;
        }
    }
}