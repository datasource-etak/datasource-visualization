using BlazorDatasource.Server.Models.Common;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Shared
{
    public partial class Pager : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnParametersSetAsync()
        {
            //min allowed page size is 1
            PageSize = Math.Max(PageSize, 1);
            TotalPages = TotalItems / PageSize;

            if (TotalItems % PageSize > 0)
                TotalPages++;

            await base.OnParametersSetAsync();
        }

        #region Properties

        /// <summary>
        /// The current page index (starts from 0)
        /// </summary>
        protected int PageIndex
        {
            get
            {
                if (PageNumber > 0)
                    return PageNumber - 1;

                return 0;
            }
        }

        /// <summary>
        /// The current page number (starts from 1)
        /// </summary>
        [Parameter]
        public int PageNumber { get; set; }

        /// <summary>
        /// Callback when page number value changed
        /// </summary>
        [Parameter]
        public EventCallback<int> PageNumberChanged { get; set; }

        /// <summary>
        /// Callback to request refresh of the table
        /// </summary>
        [Parameter]
        public EventCallback RefreshRequestedHandler { get; set; }

        /// <summary>
        /// Update the page number
        /// </summary>
        /// <param name="selectedPageNumber">Selected page number</param>
        /// <returns></returns>
        async Task UpdatePageNumber(int selectedPageNumber)
        {
            PageNumber = selectedPageNumber;
            await PageNumberChanged.InvokeAsync(PageNumber);
            await RefreshRequestedHandler.InvokeAsync();
        }

        /// <summary>
        /// The number of items in each page.
        /// </summary>
        [Parameter]
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items.
        /// </summary>
        [Parameter]
        public int TotalItems { get; set; }

        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// The index of the first item in the page.
        /// </summary>
        public int FirstItem => (PageIndex * PageSize) + 1;

        /// <summary>
        /// The index of the last item in the page.
        /// </summary>
        public int LastItem => Math.Min(TotalItems, ((PageIndex * PageSize) + PageSize));

        /// <summary>
        /// Whether there are pages before the current page.
        /// </summary>
        protected bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        /// Whether there are pages after the current page.
        /// </summary>
        protected bool HasNextPage => PageIndex + 1 < TotalPages;

        #endregion

        #region Helper functions

        /// <summary>
        /// Get first individual page index
        /// </summary>
        /// <returns>Page index</returns>
        protected int GetFirstIndividualPageIndex()
        {
            if ((TotalPages < PagingDefaults.IndividualPagesDisplayedCount) || ((PageIndex - (PagingDefaults.IndividualPagesDisplayedCount / 2)) < 0))
                return 0;

            if ((PageIndex + (PagingDefaults.IndividualPagesDisplayedCount / 2)) >= TotalPages)
                return TotalPages - PagingDefaults.IndividualPagesDisplayedCount;

            return PageIndex - (PagingDefaults.IndividualPagesDisplayedCount / 2);
        }

        /// <summary>
        /// Get last individual page index
        /// </summary>
        /// <returns>Page index</returns>
        protected int GetLastIndividualPageIndex()
        {
            var num = PagingDefaults.IndividualPagesDisplayedCount / 2;
            var mod = PagingDefaults.IndividualPagesDisplayedCount % 2;
            if (mod == 0)
                num--;
                
            if ((TotalPages < PagingDefaults.IndividualPagesDisplayedCount) || ((PageIndex + num) >= TotalPages))
                return TotalPages - 1;

            if ((PageIndex - (PagingDefaults.IndividualPagesDisplayedCount / 2)) < 0)
                return PagingDefaults.IndividualPagesDisplayedCount - 1;

            return PageIndex + num;
        }

        #endregion
    }
}
