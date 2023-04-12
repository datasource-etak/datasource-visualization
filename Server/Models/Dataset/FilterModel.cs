using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Server.Models.Dataset
{
    public partial record FilterModel
    {
        [Required]
        [JsonPropertyName("fid")]
        public FilterIdentifierModel FilterIdentifier { get; set; } = default!;

        [Required]
        [JsonPropertyName("name")]
        public string FilterName { get; set; } = default!;

        [Required]
        [JsonPropertyName("values")]
        public List<FilterValueModel> AvailableFilterValues { get; set; } = new();
    }

    /// <summary>
    /// Represents a filter result "identifier"
    /// </summary>
    public record FilterIdentifierModel
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [Required]
        [JsonPropertyName("index")]
        public int Index { get; set; } = default!;
    }

    public record FilterValueModel
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [Required]
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
    }
}
