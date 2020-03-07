using Penguin.Cms.Pages;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Penguin.Cms.Modules.Pages.Areas.Admin.Models
{
    [SuppressMessage("Design", "CA1056:Uri properties should not be strings")]
    [SuppressMessage("Design", "CA1054:Uri parameters should not be strings")]
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
    public class PageTreeDirectory
    {
        private const string NULL_PAGE_URL_MESSAGE = "Page Url can not be null";
        public List<PageTreeDirectory> Children { get; }
        public Page Page { get; set; }
        public string Url { get; set; }

        public PageTreeDirectory(Page page, List<PageTreeDirectory>? children = null) : this(page, page?.Url ?? throw new System.Exception(NULL_PAGE_URL_MESSAGE), children)
        {
        }

        public PageTreeDirectory(Page page, string url, List<PageTreeDirectory>? children = null)
        {
            this.Page = page;
            this.Url = url;
            this.Children = children ?? new List<PageTreeDirectory>();
        }
    }
}