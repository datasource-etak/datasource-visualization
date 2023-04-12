using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a search request
    /// </summary>
    public class SearchRequest
    {
        [Required]
        [JsonPropertyName("q")]
        public string Keyword { get; set; } = default!;
    }
}
