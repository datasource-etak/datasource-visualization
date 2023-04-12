using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a sign up request
    /// </summary>
    public class SignUpRequest
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; } = default!;

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;

        [Required]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = default!;

        [Required]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = default!;

        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;
    }
}
