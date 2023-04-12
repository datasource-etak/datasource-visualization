using Azure.Core;
using BlazorDatasource.Shared.Infrastructure;
using BlazorDatasource.Shared.Infrastructure.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Infrastructure
{
    /// <summary>
    /// Represents the HTTP client to request our own Datasource api found at project BlazorDatasource.Api
    /// </summary>
    public partial class DatasourceApiHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly TokenProvider _tokenProvider;

        #endregion

        #region Ctor

        public DatasourceApiHttpClient(HttpClient client,
                                       TokenProvider tokenProvider)
        {
            _httpClient = client;
            _tokenProvider = tokenProvider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create account
        /// </summary>
        /// <param name="model">SignUpRequest</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<HttpResponseMessage> CreateAccount(SignUpRequest model)
        {
            return await _httpClient.PostAsJsonAsync(requestUri: Constants.ApiRoutePaths.Register, model);
        }

        /// <summary>
        /// Sign in with account
        /// </summary>
        /// <param name="model">SignInRequest</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<ServiceResponse<SignInResponse?>> SignInAccount(SignInRequest model)
        {
            var result = await _httpClient.PostAsJsonAsync(requestUri: Constants.ApiRoutePaths.Authorize, value: model);
            if (result.IsSuccessStatusCode)
            {
                return new ServiceResponse<SignInResponse?>()
                {
                    Data = await result.Content.ReadFromJsonAsync<SignInResponse>(),
                    Success = result.IsSuccessStatusCode,
                    Message = result.ReasonPhrase ?? string.Empty
                };
            }
            else
            {
                return new ServiceResponse<SignInResponse?>()
                {
                    Data = null,
                    Success = result.IsSuccessStatusCode,
                    Message = result.ReasonPhrase ?? string.Empty
                };
            }

        }

        /// <summary>
        /// Search datasets
        /// </summary>
        /// <param name="model">SearchRequest</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<HttpResponseMessage> SearchDatasets(SearchRequest model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
            var result = await _httpClient.PostAsJsonAsync(requestUri: Constants.ApiRoutePaths.SearchDataset, value: model);
            return result;
        }

        /// <summary>
        /// Get the filters for a specific dataset result from the search
        /// </summary>
        /// <param name="model">SearchFiltersRequest</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<HttpResponseMessage> GetSearchFilters(SearchFiltersRequest model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
            var result = await _httpClient.PostAsJsonAsync(requestUri: Constants.ApiRoutePaths.GetDatasetFilters, value: model);
            return result;
        }

        /// <summary>
        /// Downloads a dataset
        /// </summary>
        /// <param name="model">DownloadDatasetRequest</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<HttpResponseMessage> DownloadDataset(DownloadDatasetRequest model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
            var result = await _httpClient.PostAsJsonAsync(requestUri: Constants.ApiRoutePaths.DownloadDataset, value: model);
            return result;
        }

        /// <summary>
        /// Gets all the downloaded datasets for a specific user
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<HttpResponseMessage> GetAllDownloadedDatasets()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
            var result = await _httpClient.PostAsync(requestUri: Constants.ApiRoutePaths.AllDatasets,content:null);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<HttpResponseMessage> GetDatasetByUUId(ViewDatasetRequest model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
            var result = await _httpClient.PostAsJsonAsync(requestUri: Constants.ApiRoutePaths.ViewDataset, value: model);
            return result;
        }

        #endregion
    }
}
