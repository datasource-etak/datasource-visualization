using System.Collections.Generic;

namespace BlazorDatasource.Shared.Infrastructure
{
    public static class Constants
    {
        public static class ApiRoutePaths
        {
            public const string DiscoveryConfiguration = ".well-known/openid-configuration";

            public const string AccountPathPrefix = "account";

            public const string Authorize = AccountPathPrefix + "/authorize";
            public const string Register = AccountPathPrefix + "/register";

            public const string DatasetPathPrefix = "dataset";

            public const string AllDatasets = DatasetPathPrefix + "/all";
            public const string AllDatasetsForSpecificFamily = DatasetPathPrefix + "/family";
            public const string ViewDataset = DatasetPathPrefix + "/view";

            public const string SearchDataset = DatasetPathPrefix + "/search";
            public const string GetDatasetFilters = DatasetPathPrefix + "/search/filters";
            public const string DownloadDataset = DatasetPathPrefix + "/download";
        }

        public static class TokenTypes
        {
            public const string RefreshToken = "refresh_token";
            public const string AccessToken = "access_token";
        }

        public static readonly List<string> SupportedTokenTypes = new()
        {
            TokenTypes.RefreshToken,
            TokenTypes.AccessToken
        };
    }
}
