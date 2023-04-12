using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a filter result after search filters for a specific dataset
    /// </summary>
    public class SearchFiltersResultFilter
    {
        [Required]
        [JsonPropertyName("fid")]
        public FilterIdentifier FilterIdentifier { get; set; } = default!;

        [Required]
        [JsonPropertyName("name")]
        public string FilterName { get; set; } = default!;

        [Required]
        [JsonPropertyName("values")]
        public List<FilterValue> AvailableFilterValues { get; set; } = new();
    }

    /// <summary>
    /// Represents a filter result "identifier"
    /// </summary>
    public class FilterIdentifier
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [Required]
        [JsonPropertyName("index")]
        public int Index { get; set; } = default!;
    }

    public class FilterValue
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [Required]
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
    }
}