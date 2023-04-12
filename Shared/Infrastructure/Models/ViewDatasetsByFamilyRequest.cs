using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a view datasets by family request
    /// </summary>
    public class ViewDatasetsByFamilyRequest
    {
        [Required]
        [JsonPropertyName("dataset_family_id")]
        public string DatasetFamilyId { get; set; } = default!;
    }
}
