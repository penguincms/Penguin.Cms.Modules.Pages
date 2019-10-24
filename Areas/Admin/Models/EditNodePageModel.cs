using Penguin.Cms.Abstractions;
using Penguin.Cms.Pages;
using Penguin.Cms.Web.Modules;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Penguin.Cms.Modules.Pages.Areas.Admin.Models
{
    [SuppressMessage("Design", "CA1056:Uri properties should not be strings")]
    [SuppressMessage("Design", "CA1054:Uri parameters should not be strings")]
    public class EditNodePageModel
    {
        public string? BaseUrl { get; set; }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
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

        private List<Macro>? _macros;

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