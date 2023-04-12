using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;

namespace BlazorDatasource.Server.Shared
{
    public partial class MainLayoutNavMenu : ComponentBase
    {
        [Inject]
        protected LinkGenerator LinkGenerator { get; set; } = default!;

        protected bool CollapseNavMenu = true;

        protected string? NavMenuCssClass => CollapseNavMenu ? "hidden" : null;

        protected void ToggleNavMenu()
        {
            CollapseNavMenu = !CollapseNavMenu;
        }
    }
}