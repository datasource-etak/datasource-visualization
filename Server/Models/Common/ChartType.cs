namespace BlazorDatasource.Server.Models.Common
{
    /// <summary>
    /// Defines the chart types.
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// None chart type (default!)
        /// </summary>
        None = 0,

        /// <summary>
        /// The bar chart type.
        /// </summary>
        Bar,

        /// <summary>
        /// The horizontal bar chart type.
        /// </summary>
        HorizontalBar,

        /// <summary>
        /// The line chart type.
        /// </summary>
        Line,

        /// <summary>
        /// The pie chart type.
        /// </summary>
        Pie,

        /// <summary>
        /// The doughnut chart type.
        /// </summary>
        Doughnut,

        /// <summary>
        /// The area chart type.
        /// </summary>
        Area,

        /// <summary>
        /// The scatter chart type.
        /// </summary>
        Scatter
    }
}
