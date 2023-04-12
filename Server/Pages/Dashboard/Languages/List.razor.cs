using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Server.Models.Localization;
using BlazorDatasource.Shared.Services.Localization;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Languages
{
    public partial class List : ComponentBase
    {
        [Inject]
        protected ILanguageService LanguageService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected EditSuccess EditSuccessState { get; set; } = default!;

        /// <summary>
        /// Languages
        /// </summary>
        protected List<LanguageOverviewModel>? Languages { get; set; }

        /// <summary>
        /// Avoid multiple concurrent requests when loading.
        /// </summary>
        protected bool Loading { get; set; }

        /// <summary>
        /// Avoid multiple concurrent requests when loading.
        /// </summary>
        protected bool Busy;

        /// <summary>
        /// Component initialization
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task OnInitializedAsync()
        {
            Loading = true;

            Languages = new List<LanguageOverviewModel>();

            Languages.AddRange((await LanguageService.GetAllLanguagesAsync(showHidden: true)).Select(language => new LanguageOverviewModel()
            {
                Id = language.Id,
                Name = language.Name,
                FlagImageFileName = $"flags/{language.FlagImageFileName}",
                Published = language.Published,
                LanguageCulture = language.LanguageCulture,
                UniqueSeoCode = language.UniqueSeoCode
            }));

            Loading = false;
        }

        /// <summary>
        /// Reload page
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ReloadAsync()
        {
            if (Loading)
            {
                return;
            }

            if (Busy)
            {
                return;
            }

            Loading = true;

            Languages = new List<LanguageOverviewModel>();

            Languages = (await LanguageService.GetAllLanguagesAsync(showHidden: true)).Select(language => new LanguageOverviewModel()
            {
                Id = language.Id,
                Name = language.Name,
                FlagImageFileName = $"flags/{language.FlagImageFileName}",
                Published = language.Published,
                LanguageCulture = language.LanguageCulture,
                UniqueSeoCode = language.UniqueSeoCode
            }).ToList();

            Loading = false;
        }

        /// <summary>
        /// Toggle published a language
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task PublishLanguageAsync(int languageId)
        {
            Busy = true;

            var language = await LanguageService.GetLanguageByIdAsync(languageId);

            if (language is not null)
            {
                language.Published = !language.Published;
                await LanguageService.UpdateLanguageAsync(language);
            }

            Busy = false;

            await ReloadAsync();
        }
    }
}
