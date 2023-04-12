using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a search filters request
    /// </summary>
    public class SearchFiltersRequest
    {
        [Required]
        [JsonPropertyName("source_id")]
        public string SourceId { get; set; } = default!;

        [Required]
        [JsonPropertyName("dataset_id")]
        public string DatasetId { get; set; } = default!;
    }
}
