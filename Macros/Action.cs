using Microsoft.AspNetCore.Mvc;
using Penguin.Cms.Abstractions;
using Penguin.Cms.Abstractions.Interfaces;
using Penguin.Cms.Core.Attributes;
using Penguin.Extensions.String;
using Penguin.Messaging.Abstractions.Interfaces;
using Penguin.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Penguin.Cms.Modules.Pages.Macros
{
    public class Action : IMessageHandler<Penguin.Messaging.Application.Messages.Startup>, IMacroProvider
    {
        private static readonly List<Macro> ControllerMacros = new();

        public void AcceptMessage(Penguin.Messaging.Application.Messages.Startup message)
        {
            Refresh();
        }

        public List<Macro> GetMacros(object requester)
        {
            return ControllerMacros;
        }

        private void Refresh()
        {
            lock (ControllerMacros)
            {
                ControllerMacros.Clear();

                IEnumerable<Type> Controllers = TypeFactory.GetDerivedTypes(typeof(Controller));

                foreach (Type thisController in Controllers)
                {
                    foreach (MethodInfo thisMethod in thisController.GetMethods())
                    {
                        bool ValidMethod = true;

                        if (thisMethod.ReturnType != typeof(ActionResult) || thisMethod.GetCustomAttribute<ShowMacroAttribute>() == null)
                        {
                            continue;
                        }

                        string Parameters = string.Join(", ", thisMethod.GetParameters().Select(p => $"{p.ParameterType.FullName} {p.Name}"));
                        Macro thisMacro = new(
                            GetType().Name,
                            $"@Html.Action(\"{thisMethod.Name}\", \"{thisController.Name.Remove("Controller")}\", \"\", {Parameters})"
                        );

                        if (ValidMethod)
                        {
                            ControllerMacros.Add(thisMacro);
                        }
                    }
                }
            }
        }
    }
}