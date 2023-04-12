using BlazorDatasource.Shared.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Server.Models.Dataset
{
    /// <summary>
    /// Represents a dataset result when a user view his downloaded datasets
    /// </summary>
    public partial record DownloadedDatasetModel : BaseModel
    {
        [Required]
        [JsonPropertyName("tuple")]
        public List<DatasetProperty> Properties { get; set; } = new();
    }
}
