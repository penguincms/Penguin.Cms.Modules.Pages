using Penguin.Cms.Abstractions;
using Penguin.Cms.Abstractions.Interfaces;
using Penguin.Cms.Web.Macros;
using System.Collections.Generic;

namespace Penguin.Cms.Modules.Pages.Macros
{
    /// <summary>
    /// This class doesn't do anything aside from allow for the Field macro to have a backing type
    /// Actual macro code is handled in the PageController and PageService
    /// </summary>
    public class Field : IMacroProvider
    {
        public List<Macro> GetMacros(object o) => new List<Macro>();
    }
}