using BlazorDatasource.Server.Infrastructure;
using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Server.Models.Dataset;
using BlazorDatasource.Server.Models.Extensions;
using BlazorDatasource.Shared.Infrastructure;
using BlazorDatasource.Shared.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Datasets
{
    public partial class Search : ComponentBase
    {
        [Inject]
        protected DatasourceApiHttpClient Client { get; set; } = default!;

        [Inject]
        protected TokenProvider TokenProvider { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected StateContainer StateContainer { get; set; } = default!;

        /// <summary>
        /// Search keyword
        /// </summary>
        public string Keyword { get; set; } = default!;

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
        /// Search results
        /// </summary>
        protected DatasetListModel? Results { get; set; }

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
        /// Triggered for any paging update.
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
            if (Loading || Page < 1)
            {
                return;
            }

            if (Busy)
            {
                return;
            }

            Loading = true;

            //get search results
            if (!string.IsNullOrEmpty(Keyword))
            {
                var remoteSearchRequest = new SearchRequest()
                {
                    Keyword = Keyword
                };

                var remoteSearchResult = await Client.SearchDatasets(remoteSearchRequest);
                if (remoteSearchResult.IsSuccessStatusCode)
                {
                    var searchResults = await remoteSearchResult.Content.ReadFromJsonAsync<List<DatasetModel>>();
                    if (searchResults is null)
                    {
                        return;
                    }

                    var searchResultsQueryable = searchResults.AsQueryable();
                    var pagedSearchResults = searchResultsQueryable.ToPagedList(Page - 1, PageSize);

                    //prepare list model
                    Results = new DatasetListModel().PrepareToGrid(pagedSearchResults, () =>
                    {
                        //fill in model values from the entity
                        return pagedSearchResults.Select(dataset => new DatasetModel
                        {
                            DatasetId = dataset.DatasetId,
                            DatasetName = dataset.DatasetName,
                            DatasetDescription = dataset.DatasetDescription,
                            SourceId = dataset.SourceId,
                            SourceName = dataset.SourceName
                        });
                    });
                }
                else
                {
                    Error = true;
                    ErrorMessage = remoteSearchResult.ReasonPhrase ?? T["Datasets.Search.Error"];
                }
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
        /// Called on keyup on search
        /// </summary>
        /// <param name="ev">Keyboards events</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task SearchKeyPress(KeyboardEventArgs ev)
        {
            if (ev.Key == "Enter")
            {
                await ReloadAsync();
            }
        }

        /// <summary>
        /// Navigate to filters
        /// </summary>
        /// <param name="selectedDataset">Selected Dataset result</param>
        protected void ViewFilters(DatasetModel selectedDataset)
        {
            StateContainer.Dataset = selectedDataset;
            NavigationManager.NavigateTo($"/dashboard/datasets/search/filters/");
        }
    }
}
