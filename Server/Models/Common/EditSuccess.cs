namespace BlazorDatasource.Server.Models.Common
{
    /// <summary>
    /// Service to communicate success status between pages.
    /// </summary>
    public class EditSuccess
    {
        /// <summary>
        /// <c>true</c> when the last edit operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message when the last edit operation was successful.
        /// </summary>
        public string SuccessMessage { get; set; } = default!;
    }
}