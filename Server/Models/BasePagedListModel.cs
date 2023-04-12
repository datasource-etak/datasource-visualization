using System.Collections.Generic;

namespace BlazorDatasource.Server.Models
{
    /// <summary>
    /// Represents the base paged list model
    /// </summary>
    public abstract partial record BasePagedListModel<T> : BaseModel, IPagedModel<T> where T : BaseModel
    {
        /// <summary>
        /// Gets or sets data records
        /// </summary>
        public IEnumerable<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// Gets or sets a number of filtered data records
        /// </summary>
        public int RecordsFiltered { get; set; }

        /// <summary>
        /// Gets or sets a number of total data records
        /// </summary>
        public int RecordsTotal { get; set; }
    }
}
