﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;

namespace BlazorDatasource.Server.Shared
{
    public partial class DashboardLayoutHeader : ComponentBase
    {
        [Inject]
        protected LinkGenerator LinkGenerator { get; set; } = default!;
    }
}
