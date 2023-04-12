using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a timeline result when a user view a specific dataset
    /// </summary>
    public class ResultTimeline
    {
        [Required]
        [JsonPropertyName("tuple")]
        public List<TimelineProperty> Properties { get; set; } = new();
    }

    /// <summary>
    /// Represents a timeline property
    /// </summary>
    public class TimelineProperty
    {
        [Required]
        [JsonPropertyName("key")]
        public string Key { get; set; } = default!;

        [Required]
        [JsonPropertyName("value")]
        public string Value { get; set; } = default!;
    }
}
