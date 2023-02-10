using Penguin.Cms.Abstractions;
using Penguin.Cms.Pages;
using Penguin.Cms.Web.Modules;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Cms.Modules.Pages.Areas.Admin.Models
{
    public class EditNodePageModel
    {
        private List<Macro>? _macros;
        public string? BaseUrl { get; set; }

        public List<Macro> Macros
        {
            get
            {
                List<Macro> toReturn = new();

                if (Page != null)
                {
                    toReturn.AddRange(Page.Parameters.Select(p => new Macro("Field", $"@Model.{p.Name}")).ToList());
                }

                if (_macros != null)
                {
                    toReturn.AddRange(_macros);
                }

                return toReturn;
            }
            set => _macros = value;
        }

        public ICollection<ViewModule> Modules { get; set; } = new List<ViewModule>();
        public Page? Page { get; set; }

        public EditNodePageModel()
        {
        }

        public EditNodePageModel(string baseUrl, Page? page = null)
        {
            Page = page ?? new Page();
            BaseUrl = baseUrl;
        }
    }
}