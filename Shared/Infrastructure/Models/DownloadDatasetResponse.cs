using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a download dataset uuid response 
    /// </summary>
    public class DownloadDatasetResponse
    {
        [Required]
        [JsonPropertyName("uuid")]
        public string DatasetUUId { get; set; } = default!;
    }
}
