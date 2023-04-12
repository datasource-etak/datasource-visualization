using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a view dataset request
    /// </summary>
    public class ViewDatasetRequest
    {
        [Required]
        [JsonPropertyName("uuid")]
        public string DatasetUUId { get; set; } = default!;
    }
}
