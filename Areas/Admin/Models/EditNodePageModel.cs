using Penguin.Cms.Abstractions;
using Penguin.Cms.Pages;
using Penguin.Cms.Web.Modules;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
                List<Macro> toReturn = new List<Macro>();

                if (this.Page != null)
                {
                    toReturn.AddRange(this.Page.Parameters.Select(p => new Macro("Field", $"@Model.{p.Name}")).ToList());
                }

                if (this._macros != null)
                {
                    toReturn.AddRange(this._macros);
                }

                return toReturn;
            }
            set => this._macros = value;
        }

        public ICollection<ViewModule> Modules { get; set; } = new List<ViewModule>();
        public Page? Page { get; set; }

        public EditNodePageModel()
        {
        }

        public EditNodePageModel(string baseUrl, Page? page = null)
        {
            this.Page = page ?? new Page();
            this.BaseUrl = baseUrl;
        }
    }
}