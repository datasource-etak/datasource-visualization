using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a Dataset result after search
    /// </summary>
    public class SearchResultDataset
    {
        [Required]
        [JsonPropertyName("dataset_id")]
        public string DatasetId { get; set; } = default!;

        [Required]
        [JsonPropertyName("dataset_name")]
        public string DatasetName { get; set; } = default!;

        [Required]
        [JsonPropertyName("dataset_description")]
        public string DatasetDescription { get; set; } = default!;

        [Required]
        [JsonPropertyName("source_id")]
        public string SourceId { get; set; } = default!;

        [Required]
        [JsonPropertyName("source_name")]
        public string SourceName { get; set; } = default!;
    }
}