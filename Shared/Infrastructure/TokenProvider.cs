namespace BlazorDatasource.Shared.Infrastructure
{
    public class TokenProvider
    {
        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; } = default!;

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; } = default!;
    }
}
