using BlazorDatasource.Server.Infrastructure;
using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Server.Models.Dataset;
using BlazorDatasource.Server.Models.Extensions;
using BlazorDatasource.Shared.Infrastructure;
using BlazorDatasource.Shared.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Datasets
{
    public partial class View : ComponentBase
    {
        [Parameter]
        public string? DownloadedDatasetUUId { get; set; }

        [Parameter]
        public string? DownloadedDatasetName { get; set; }

        [Inject]
        protected DatasourceApiHttpClient Client { get; set; } = default!;

        [Inject]
        protected TokenProvider TokenProvider { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected EditSuccess EditSuccessState { get; set; } = default!;

        /// <summary>
        /// Gets or sets the default chart type for the selected dataset
        /// </summary>
        protected ChartType DefaultChartType { get; set; } = default!;

        /// <summary>
        /// Gets or sets whether we should render the properties for each timeline found or not
        /// </summary>
        protected bool RenderProperties { get; set; } = false;

        /// <summary>
        /// Gets or sets whether we should render the graphs or not
        /// </summary>
        protected bool DisplayCharts { get; set; } = true;

        /// <summary>
        /// Represents the chart data (KeyValuePair of time property and value property)
        /// </summary>
        protected List<KeyValuePair<string, decimal>> ChartData { get; set; } = new();

        /// <summary>
        /// Timeline results
        /// </summary>
        protected DatasetTimelineListModel? Results { get; set; }

        /// <summary>
        /// Gets or sets a page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets a page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of available page sizes
        /// </summary>
        public string? AvailablePageSizes { get; set; }

        /// <summary>
        /// Avoid multiple concurrent requests when loading.
        /// </summary>
        protected bool Loading { get; set; }

        /// <summary>
        /// Avoid concurrent requests
        /// </summary>
        protected bool Busy;

        /// <summary>
        /// An error occurred in the update
        /// </summary>
        protected bool Error;

        /// <summary>
        /// Error message
        /// </summary>
        protected string ErrorMessage = string.Empty;

        /// <summary>
        /// Start it up
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task OnInitializedAsync()
        {
            // Paging
            Page = 1;
            PageSize = PagingDefaults.DefaultGridPageSize;
            AvailablePageSizes = PagingDefaults.GridPageSizes;

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Get the selected dataset
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task OnParametersSetAsync()
        {
            await ReloadAsync();
            await base.OnParametersSetAsync();
        }

        /// <summary>
        /// Reload page
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ReloadAsync()
        {
            if (DownloadedDatasetUUId is null)
            {
                return;
            }

            if (Loading)
            {
                return;
            }

            if (Busy)
            {
                return;
            }

            Loading = true;

            //get the dataset data
            var remoteViewDatasetRequest = new ViewDatasetRequest()
            {
                DatasetUUId = DownloadedDatasetUUId
            };

            var remoteViewDatasetResult = await Client.GetDatasetByUUId(remoteViewDatasetRequest);
            if (remoteViewDatasetResult.IsSuccessStatusCode)
            {
                var datasetTimelineResults = await remoteViewDatasetResult.Content.ReadFromJsonAsync<List<DatasetTimelineModel>>();
                if (datasetTimelineResults is null)
                {
                    return;
                }

                var datasetTimelineResultsQueryable = datasetTimelineResults.AsQueryable();
                var pagedDatasetTimelineResults = datasetTimelineResultsQueryable.ToPagedList(Page - 1, PageSize);

                //prepare list model
                Results = new DatasetTimelineListModel().PrepareToGrid(pagedDatasetTimelineResults, () =>
                {
                    //fill in model values from the entity
                    return pagedDatasetTimelineResults.Select(datasetTimeline => new DatasetTimelineModel
                    {
                        Properties = datasetTimeline.Properties
                    });
                });

                //chart data 
                ChartData.Clear();
                foreach (var item in Results.Data)
                {
                    string? chartDataTupleKey = null;
                    decimal chartDataTupleValue = decimal.Zero;
                    foreach (var property in item.Properties)
                    {
                        if (property.Key == "time")
                        {
                            chartDataTupleKey = property.Value;
                        }
                        else if (property.Key == "value")
                        {
                            decimal.TryParse(property.Value, System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.AllowDecimalPoint, null, out chartDataTupleValue);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    ChartData.Add(new KeyValuePair<string, decimal>(chartDataTupleKey ?? "", chartDataTupleValue));
                }

                ChartData = ChartData.OrderBy(keyValuePair => keyValuePair.Key).ToList();
            }
            else
            {
                Error = true;
                ErrorMessage = remoteViewDatasetResult.ReasonPhrase ?? T["Datasets.Downloaded.View.Error"];
            }

            Loading = false;
        }

        /// <summary>
        /// OnPageSizeChanged event
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task OnPageSizeChanged(ChangeEventArgs args)
        {
            if (args.Value is not null)
            {
                Page = 1;
                PageSize = Convert.ToInt32(args.Value);
                await ReloadAsync();
            }
        }

        /// <summary>
        /// Show the graphs
        /// </summary>
        protected void ToggleDisplayCharts()
        {
            DisplayCharts = !DisplayCharts;
        }

        /// <summary>
        /// Handle a chart type selection from a chart blazor component
        /// </summary>
        /// <param name="selectedChartType">Selected chart type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task HandleDefaultChartTypeSelection(ChartType selectedChartType)
        {
            Busy = true;

            await Task.Delay(4000);

            if (DefaultChartType == selectedChartType)
            {
                DefaultChartType = default!;
            }
            else
            {
                DefaultChartType = selectedChartType;
            }

            Busy = false;
        }
    }
}
