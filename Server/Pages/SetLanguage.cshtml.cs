using BlazorDatasource.Shared;
using BlazorDatasource.Shared.Services.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages
{
    public class SetLanguageModel : PageModel
    {
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

        public int LanguageId { get; set; }

        public SetLanguageModel(ILanguageService languageService,
                                IWorkContext workContext,
                                IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _languageService = languageService;
            _workContext = workContext;
            _localizationOptions = localizationOptions;
        }

        public async Task<IActionResult> OnGetAsync(int languageId, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            LanguageId = languageId;

            var language = await _languageService.GetLanguageByIdAsync(LanguageId);
            if (!language?.Published ?? false)
                language = await _workContext.GetWorkingLanguageAsync();

            if (language is not null)
            {
                var selectedLanguageRequestCulture = new RequestCulture(language.LanguageCulture, language.LanguageCulture);
                var selectedLanguageIsAlreadySupported = _localizationOptions.Value.SupportedCultures?.Contains(selectedLanguageRequestCulture.Culture);

                if(selectedLanguageIsAlreadySupported is false)
                {
                    _localizationOptions.Value.SupportedCultures?.Add(selectedLanguageRequestCulture.Culture);
                }

                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(selectedLanguageRequestCulture));
            }

            return LocalRedirect(returnUrl);
        }
    }
}
