using Penguin.Cms.Modules.Core.ComponentProviders;
using Penguin.Cms.Modules.Core.Navigation;
using Penguin.Cms.Modules.Pages.Constants.Strings;
using Penguin.Navigation.Abstractions;
using Penguin.Security.Abstractions;
using Penguin.Security.Abstractions.Interfaces;
using System.Collections.Generic;

namespace Penguin.Cms.Modules.Pages.ComponentProviders
{
    public class AdminNavigationMenuProvider : NavigationMenuProvider
    {
        public override INavigationMenu GenerateMenuTree()
        {
            return new NavigationMenu()
            {
                Name = "Admin",
                Text = "Admin",
                Children = new List<INavigationMenu>() {
                        new NavigationMenu()
                        {
                            Text = "Pages",
                            Name = "PagesAdmin",
                            Href = "/Admin/Page/Index",
                            Permissions = new List<ISecurityGroupPermission>()
                            {
                                this.CreatePermission(RoleNames.ContentManager, PermissionTypes.Read)
                            }
                        }
                    }
            };
        }
    }
}