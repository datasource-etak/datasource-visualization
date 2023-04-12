using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a sign in request
    /// </summary>
    public class SignInRequest
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; } = default!;

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;
    }
}
