using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;

namespace BlazorDatasource.Server.Shared
{
    public partial class AccessDenied : ComponentBase
    {
        [Inject]
        protected LinkGenerator LinkGenerator { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
    }
}
