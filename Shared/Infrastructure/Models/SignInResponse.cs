using System.Text.Json.Serialization;

namespace BlazorDatasource.Shared.Infrastructure.Models
{
    /// <summary>
    /// Represents a sign in response
    /// </summary>
    public class SignInResponse
    {
        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = default!;

        /// <summary>
        /// Gets or sets the expiration time of the access token
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of the refresh token
        /// </summary>
        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the refresh token
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = default!;

        /// <summary>
        /// Gets or sets the token type
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = default!;

        /// <summary>
        /// Gets or sets the id token
        /// </summary>
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = default!;

        /// <summary>
        /// Gets or set the not before policy (I have no idea what that is)
        /// </summary>
        [JsonPropertyName("notbeforepolicy")]
        public int NotBeforePolicy { get; set; }

        /// <summary>
        /// Gets or sets the session state
        /// </summary>
        [JsonPropertyName("session_state")]
        public string SessionState { get; set; } = default!;

        /// <summary>
        /// Gets or sets the scope
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = default!;
    }
}
