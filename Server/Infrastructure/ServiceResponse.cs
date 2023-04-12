using System.Text.Json.Serialization;

namespace BlazorDatasource.Server.Infrastructure
{
    /// <summary>
    /// Represents the generic response our api returns
    /// </summary>
    /// <typeparam name="T">The "data" that will be returned from our api calls</typeparam>
    public class ServiceResponse<T>
    {
        /// <summary>
        /// Contains the data to return as response
        /// </summary>
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;

        /// <summary>
        /// Gets or sets if api call succeeded or not
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; } = true;

        /// <summary>
        /// Contains the message sent
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;
    }
}
