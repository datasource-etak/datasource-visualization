namespace BlazorDatasource.Shared.Services.Localization
{
    /// <summary>
    /// Represents default values related to localization services
    /// </summary>
    public static partial class LocalizationDefaults
    {
        /// <summary>
        /// Gets a value indicating if we should auto create the locale string resource if it was not found
        /// </summary>
        public const bool CreateLocaleStringResourceIfNotFound = true;

        /// <summary>
        /// Gets a prefix of locale resources for enumerations 
        /// </summary>
        public const string EnumLocaleStringResourcesPrefix = "Enums.";
    }
}
