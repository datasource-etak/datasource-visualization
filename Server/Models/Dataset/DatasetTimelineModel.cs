using BlazorDatasource.Shared.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Server.Models.Dataset
{
    public partial record DatasetTimelineModel : BaseModel
    {
        [Required]
        [JsonPropertyName("tuple")]
        public List<TimelineProperty> Properties { get; set; } = new();
    }
}
