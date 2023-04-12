using BlazorDatasource.Server.Infrastructure;
using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Server.Models.Dataset;
using BlazorDatasource.Shared.Infrastructure;
using BlazorDatasource.Shared.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Datasets
{
    public partial class Filters : ComponentBase
    {
        [Inject]
        protected DatasourceApiHttpClient Client { get; set; } = default!;

        [Inject]
        protected TokenProvider TokenProvider { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected StateContainer StateContainer { get; set; } = default!;

        [Inject]
        protected EditSuccess EditSuccessState { get; set; } = default!;

        /// <summary>
        /// Represents the available filters for the selected dataset from the search
        /// </summary>
        protected List<FilterModel> AvailableFilters { get; set; } = new();

        /// <summary>
        /// Gets or sets the search filter value
        /// </summary>
        protected string SearchFilterValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Filtered filter values according to search filter value above
        /// It does the search when we type at the search box
        /// </summary>
        protected List<FilterValueModel> Filtered => AvailableFilters.SelectMany(filter => filter.AvailableFilterValues)
                                                                     .Where(filterValue => filterValue.Description.ToLowerInvariant()
                                                                     .Contains(SearchFilterValue.ToLowerInvariant()))
                                                                     .OrderBy(filterValue => filterValue.Description)
                                                                     .ToList();

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
            if (StateContainer.Dataset is null)
            {
                return;
            }

            Loading = true;

            // get the filters
            var remoteSearchFiltersRequest = new SearchFiltersRequest()
            {
                DatasetId = StateContainer.Dataset.DatasetId,
                SourceId = StateContainer.Dataset.SourceId
            };

            var remoteSearchFiltersResult = await Client.GetSearchFilters(remoteSearchFiltersRequest);
            if (remoteSearchFiltersResult.IsSuccessStatusCode)
            {
                var searchFiltersResults = await remoteSearchFiltersResult.Content.ReadFromJsonAsync<List<FilterModel>>();
                AvailableFilters = searchFiltersResults ?? new();
            }
            else
            {
                Error = true;
                ErrorMessage = remoteSearchFiltersResult.ReasonPhrase ?? T["Datasets.Search.Filters.Error"];
            }

            Loading = false;

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Gets or sets the selected filter
        /// </summary>
        protected FilterModel? SelectedFilter { get; set; }

        /// <summary>
        /// Gets or sets the selected filter
        /// </summary>
        protected FilterValueModel? SelectedFilterValue { get; set; }

        /// <summary>
        /// Gets or sets the selected filter value
        /// </summary>
        /// <param name="selectedFilterValue"></param>
        protected void SelectFilter(FilterModel selectFilter, FilterValueModel selectFilterValue)
        {
            if (selectFilter is null)
            {
                return;
            }

            if (selectFilterValue is null)
            {
                return;
            }

            if (Busy)
            {
                return;
            }

            Busy = true;

            SelectedFilter = selectFilter;

            // we need to toggle the selected filter value
            // if there is a selected filter already
            if (SelectedFilterValue is not null && SelectedFilterValue.Id.Equals(selectFilterValue.Id, System.StringComparison.InvariantCultureIgnoreCase))
            {
                SelectedFilterValue = null;
            }
            else
            {
                SelectedFilterValue = selectFilterValue;
            }

            SearchFilterValue = string.Empty;
            Filtered.Clear();

            Busy = false;
        }

        /// <summary>
        /// Download a dataset with the selected filters
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task DownloadDataset()
        {
            if (StateContainer.Dataset is null)
            {
                return;
            }

            if (SelectedFilter is null)
            {
                return;
            }

            if (SelectedFilterValue is null)
            {
                return;
            }

            if (Busy)
            {
                return;
            }

            Busy = true;

            // prepare the download dataset request
            var remoteDownloadDatasetRequest = new DownloadDatasetRequest()
            {
                DatasetId = StateContainer.Dataset.DatasetId,
                SourceId = StateContainer.Dataset.SourceId
            };

            // prepare the selected filters to download a dataset
            SearchFiltersResultFilter selectedFilters = new()
            {
                FilterIdentifier = new FilterIdentifier()
                {
                    Id = SelectedFilter.FilterIdentifier.Id,
                    Index = SelectedFilter.FilterIdentifier.Index
                },
                FilterName = SelectedFilter.FilterName
            };

            selectedFilters.AvailableFilterValues.Clear();
            selectedFilters.AvailableFilterValues.Add(new FilterValue()
            {
                Id = SelectedFilterValue.Id,
                Description = SelectedFilterValue.Description
            });

            remoteDownloadDatasetRequest.SelectedFilters.Add(selectedFilters);

            var remoteDownloadDatasetResult = await Client.DownloadDataset(remoteDownloadDatasetRequest);
            if (remoteDownloadDatasetResult.IsSuccessStatusCode)
            {
                var downloadedDatasetUUId = await remoteDownloadDatasetResult.Content.ReadFromJsonAsync<DownloadDatasetResponse>();
                if (downloadedDatasetUUId is not null)
                {
                    EditSuccessState.Success = true;
                    EditSuccessState.SuccessMessage = T["Datasets.Search.Filters.DownloadDataset.Success"];
                    NavigationManager.NavigateTo($"/dashboard/datasets/downloaded/view/{downloadedDatasetUUId.DatasetUUId}");
                }
                else
                {
                    EditSuccessState.Success = false;
                    Error = true;
                    ErrorMessage = T["Datasets.Search.Filters.DownloadDataset.Error"];
                }
            }
            else
            {
                Error = true;
                ErrorMessage = remoteDownloadDatasetResult.ReasonPhrase ?? T["Datasets.Search.Filters.DownloadDataset.Error"];
            }

            Busy = false;
        }

        /// <summary>
        /// Clears the selected filter value
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected void ClearSelectedFilterValue()
        {
            SelectedFilterValue = null;
        }
    }
}
