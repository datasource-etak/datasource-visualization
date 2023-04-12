using BlazorDatasource.Shared.Domain.Localization;
using BlazorDatasource.Shared.Services.Localization;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDatasource.Shared
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Fields

        private readonly ILanguageService _languageService;

        private Language _cachedLanguage;

        #endregion

        #region Ctor

        public WebWorkContext(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get language from the request
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the found language
        /// </returns>
        protected virtual async Task<Language> GetLanguageFromRequestAsync()
        {
            //try to get language by culture name
            var requestLanguage = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault(language => language.LanguageCulture.Equals(CultureInfo.CurrentCulture.Name, StringComparison.InvariantCultureIgnoreCase));

            //check language availability
            if (requestLanguage == null || !requestLanguage.Published)
                return null;

            return requestLanguage;
        }

        /// <summary>
        /// Get language from the request
        /// </summary>
        /// <returns>
        /// The found language
        /// </returns>
        protected virtual Language GetLanguageFromRequest()
        {
            //try to get language by culture name
            var requestLanguage = _languageService.GetAllLanguages().FirstOrDefault(language => language.LanguageCulture.Equals(CultureInfo.CurrentCulture.Name, StringComparison.InvariantCultureIgnoreCase));

            //check language availability
            if (requestLanguage == null || !requestLanguage.Published)
                return null;

            return requestLanguage;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets current user working language
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<Language> GetWorkingLanguageAsync()
        {
            //whether there is a cached value
            if (_cachedLanguage != null)
                return _cachedLanguage;

            //try to detect the language
            Language detectedLanguage = await GetLanguageFromRequestAsync();

            //if there are no languages detected try to get the first one even if it's not published
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            detectedLanguage ??= allLanguages.FirstOrDefault();

            //there are no languages create the default one
            if (detectedLanguage is null)
            {
                var defaultLanguage = new Language()
                {
                    Name = "English",
                    UniqueSeoCode = "en",
                    LanguageCulture = "en-US",
                    FlagImageFileName = "us.png",
                    DisplayOrder = 1,
                    Published = true
                };

                try
                {
                    await _languageService.InsertLanguageAsync(defaultLanguage);
                    detectedLanguage = defaultLanguage;
                }
                catch (Exception)
                {
                }
            }

            //cache the found language
            _cachedLanguage = detectedLanguage;

            return _cachedLanguage;
        }

        /// <summary>
        /// Gets current user working language
        /// </summary>
        /// <returns>Language</returns>
        public virtual Language GetWorkingLanguage()
        {
            //whether there is a cached value
            if (_cachedLanguage != null)
                return _cachedLanguage;

            //try to detect the language
            Language detectedLanguage = GetLanguageFromRequest();

            //if there are no languages detected try to get the first one even if it's not published
            var allLanguages = _languageService.GetAllLanguages(showHidden: true);

            detectedLanguage ??= allLanguages.FirstOrDefault();

            //there are no languages create the default one
            if (detectedLanguage is null)
            {
                var defaultLanguage = new Language()
                {
                    Name = "English",
                    UniqueSeoCode = "en",
                    LanguageCulture = "en-US",
                    FlagImageFileName = "us.png",
                    DisplayOrder = 1,
                    Published = true
                };

                try
                {
                    _languageService.InsertLanguageAsync(defaultLanguage);
                    detectedLanguage = defaultLanguage;
                }
                catch (Exception)
                {
                }
            }

            //cache the found language
            _cachedLanguage = detectedLanguage;

            return _cachedLanguage;
        }

        #endregion
    }
}
