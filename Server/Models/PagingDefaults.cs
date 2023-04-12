namespace BlazorDatasource.Server.Models.Common
{
    /// <summary>
    /// Represents default values related to paging
    /// </summary>
    public static partial class PagingDefaults
    {
        /// <summary>
        /// Default grid page size
        /// </summary>
        public const int DefaultGridPageSize = 15;

        /// <summary>
        /// A comma-separated list of available grid page sizes
        /// </summary>
        public const string GridPageSizes = "7, 15, 20, 50, 100";

        /// <summary>
        /// Number of individual page items to display
        /// </summary>
        public const int IndividualPagesDisplayedCount = 5;

        /// <summary>
        /// Page query string prameter name
        /// </summary>
        public const string QueryParam = "page";
    }
}
