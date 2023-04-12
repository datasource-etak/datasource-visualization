using BlazorDatasource.Server.Models.Common;

namespace BlazorDatasource.Server.Models
{
    /// <summary>
    /// Represents base search model
    /// </summary>
    public abstract partial record BaseSearchModel : IPagingRequestModel
    {
        #region Ctor

        protected BaseSearchModel()
        {
            //set the default values
            Page = 1;
            PageSize = 10;
        }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        /// <summary>
        /// Set grid page parameters
        /// </summary>
        public void SetGridPageSize()
        {
            Page = 1;
            PageSize = PagingDefaults.DefaultGridPageSize;
            AvailablePageSizes = PagingDefaults.GridPageSizes;
        }

        #endregion
    }
}
