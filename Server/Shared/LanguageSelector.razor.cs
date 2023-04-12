using BlazorDatasource.Server.Models.Localization;
using BlazorDatasource.Shared;
using BlazorDatasource.Shared.Services.Localization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Shared
{
    public partial class LanguageSelector : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ILanguageService LanguageService { get; set; } = default!;

        [Inject]
        protected IWorkContext WorkContext { get; set; } = default!;

        protected List<LanguageSelectionModel> AvailableLanguages { get; set; } = new();

        protected LanguageSelectionModel CurrentLanguage { get; set; } = new LanguageSelectionModel();

        protected int CurrentLanguageId { get; set; }

        protected bool CollapseDropDownMenu = true;

        protected string? DropDownMenuCssClass => CollapseDropDownMenu ? "hidden" : null;

        /// <summary>
        /// Component initialization
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task OnInitializedAsync()
        {
            int currentLanguageId = (await WorkContext.GetWorkingLanguageAsync()).Id;
            var allAvailableLanguages = (await LanguageService.GetAllLanguagesAsync()).Select(x => new LanguageSelectionModel
            {
                Id = x.Id,
                Name = x.Name,
                FlagImageFileName = $"flags/{x.FlagImageFileName}",
                Selected = x.Id == currentLanguageId
            }).ToList();

            var currentSelectedLanguage = await WorkContext.GetWorkingLanguageAsync();
            if (currentSelectedLanguage is not null)
            {
                CurrentLanguage = new LanguageSelectionModel
                {
                    Id = currentSelectedLanguage.Id,
                    Name = currentSelectedLanguage.Name,
                    FlagImageFileName = $"flags/{currentSelectedLanguage.FlagImageFileName}",
                };
            }

            AvailableLanguages = allAvailableLanguages;
        }

        protected void ToggleDropDownMenu()
        {
            CollapseDropDownMenu = !CollapseDropDownMenu;
        }

        protected void SetSelectedLanguage(int langId)
        {
            var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            var uriEscaped = Uri.EscapeDataString(uri);

            NavigationManager.NavigateTo(uri: $"language/set?languageId={langId}&returnUrl={uriEscaped}",
                                         forceLoad: true);
        }
    }
}
