namespace BlazorDatasource.Shared.Infrastructure
{
    public class InitialApplicationState
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
