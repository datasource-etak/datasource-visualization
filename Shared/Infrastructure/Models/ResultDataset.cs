using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a dataset result when a user view his downloaded datasets
    /// </summary>
    public class ResultDataset
    {
        [Required]
        [JsonPropertyName("tuple")]
        public List<DatasetProperty> Properties { get; set; } = new();
    }

    /// <summary>
    /// Represents a dataset property
    /// </summary>
    public class DatasetProperty
    {
        [Required]
        [JsonPropertyName("key")]
        public string Key { get; set; } = default!;

        [Required]
        [JsonPropertyName("value")]
        public string Value { get; set; } = default!;
    }
}
