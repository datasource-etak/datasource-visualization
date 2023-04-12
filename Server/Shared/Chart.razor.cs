using BlazorDatasource.Server.Models.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Shared
{
    public partial class Chart : ComponentBase
    {
        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// Auto generated id for Chart canvas
        /// </summary>
        protected Guid ComponentInstanceId { get; } = Guid.NewGuid();

        /// <summary>
        /// Avoid concurrent requests
        /// </summary>
        [Parameter]
        public bool Busy { get; set; }

        /// <summary>
        /// Chart Type parameter
        /// </summary>
        [Parameter]
        public ChartType Type { get; set; } = default!;

        /// <summary>
        /// Represents the chart data (KeyValuePair of time property and value property)
        /// </summary>
        [Parameter]
        public List<KeyValuePair<string, decimal>> ChartData { get; set; } = new();

        /// <summary>
        /// Represents the chart title passed from parent
        /// </summary>
        [Parameter]
        public string? ChartTitle { get; set; } = default!;

        /// <summary>
        /// Let parent handle the selection of a chart type as default
        /// </summary>
        [Parameter]
        public EventCallback<ChartType> SetDefaultChartTypeCallback { get; set; }

        /// <summary>
        /// If the chart is selected as default chart view for dash board
        /// </summary>
        [Parameter]
        public bool IsDefaultDatasetViewChart { get; set; } = false;

        /// <summary>
        /// Invoke js for draw chart
        /// </summary>
        /// <param name="firstRender">true if firstRender;false otherwise</param>
        /// returns>A task that represents the asynchronous operation</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            switch (Type)
            {
                case ChartType.Bar:
                    await JS.InvokeVoidAsync("createNewBarChart", new
                    {
                        xs = ChartData.Select(pair => pair.Key).ToArray(),
                        ys = ChartData.Select(pair => pair.Value).ToArray(),
                        target = ComponentInstanceId
                    });
                    break;
                case ChartType.HorizontalBar:
                    await JS.InvokeVoidAsync("createNewHorizontalBarChart", new
                    {
                        xs = ChartData.Select(pair => pair.Key).ToArray(),
                        ys = ChartData.Select(pair => pair.Value).ToArray(),
                        target = ComponentInstanceId
                    });
                    break;
                case ChartType.Line:
                    // This function calls a JavaScript function to update the chart.
                    await JS.InvokeVoidAsync("createNewLineChart", new
                    {
                        xs = ChartData.Select(pair => pair.Key).ToArray(),
                        ys = ChartData.Select(pair => pair.Value).ToArray(),
                        target = ComponentInstanceId
                    });
                    break;
                case ChartType.Pie:
                    await JS.InvokeVoidAsync("createNewPieChart", new
                    {
                        xs = ChartData.Select(pair => pair.Key).ToArray(),
                        ys = ChartData.Select(pair => pair.Value).ToArray(),
                        target = ComponentInstanceId
                    });
                    break;
                case ChartType.Doughnut:
                    await JS.InvokeVoidAsync("createNewDoughnutChart", new
                    {
                        xs = ChartData.Select(pair => pair.Key).ToArray(),
                        ys = ChartData.Select(pair => pair.Value).ToArray(),
                        target = ComponentInstanceId
                    });
                    break;
                case ChartType.Area:
                    await JS.InvokeVoidAsync("createNewAreaChart", new
                    {
                        xs = ChartData.Select(pair => pair.Key).ToArray(),
                        ys = ChartData.Select(pair => pair.Value).ToArray(),
                        target = ComponentInstanceId
                    });
                    break;
                case ChartType.Scatter:
                    await JS.InvokeVoidAsync("createNewScatterChart", new
                    {
                        xs = ChartData.Select(pair => pair.Key).ToArray(),
                        ys = ChartData.Select(pair => pair.Value).ToArray(),
                        target = ComponentInstanceId
                    });
                    break;
                default:
                    break;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Click on chart event handler
        /// </summary>
        /// returns>A task that represents the asynchronous operation</returns>
        protected async Task ClickChartEventHandler()
        {
            if (Busy)
            {
                return;
            }

            if (SetDefaultChartTypeCallback.HasDelegate)
            {
                await SetDefaultChartTypeCallback.InvokeAsync(Type);
            }
        }
    }
}
