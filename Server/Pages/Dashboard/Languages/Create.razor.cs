using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Server.Models.Localization;
using BlazorDatasource.Shared.Domain.Localization;
using BlazorDatasource.Shared.Services.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Languages
{
    public partial class Create : ComponentBase
    {
        [Inject]
        protected ILanguageService LanguageService { get; set; } = default!;

        [Inject]
        protected IValidator<LanguageModel> Validator { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected EditSuccess EditSuccessState { get; set; } = default!;

        protected LanguageModel? LanguageModel { get; set; }

        /// <summary>
        /// Avoid multiple concurrent requests when loading.
        /// </summary>
        protected bool Busy;

        /// <summary>
        /// <c>True</c> when an error occurred.
        /// </summary>
        protected bool Error;

        /// <summary>
        /// The error message
        /// </summary>
        protected string ErrorMessage = string.Empty;

        /// <summary>
        /// Component initialization
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task OnInitializedAsync()
        {
            LanguageModel = new LanguageModel()
            {
                DisplayOrder = (await LanguageService.GetAllLanguagesAsync(showHidden: true)).Max(l => l.DisplayOrder) + 1,
                Published = true
            };

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Respond to a forms submission.
        /// </summary>
        /// <param name="success"><c>True</c> when valid.</param>
        /// <param name="continueEditing"><c>True</c> when we need to go to edit page</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ValidationResultAsync(bool success, bool continueEditing)
        {
            if (Busy)
            {
                return;
            }

            if (!success)
            {
                Error = false;
                return;
            }

            Busy = true;

            try
            {
                if (LanguageModel is not null)
                {
                    var newLanguage = new Language()
                    {
                        Name = LanguageModel.Name,
                        LanguageCulture = LanguageModel.LanguageCulture,
                        UniqueSeoCode = LanguageModel.UniqueSeoCode,
                        FlagImageFileName = LanguageModel.FlagImageFileName,
                        DisplayOrder = LanguageModel.DisplayOrder,
                        Published = LanguageModel.Published
                    };
                    await LanguageService.InsertLanguageAsync(newLanguage);
                    EditSuccessState.Success = true;
                    EditSuccessState.SuccessMessage = T["Languages.Added"];

                    if (!continueEditing)
                        NavigationManager.NavigateTo("/dashboard/languages");
                    else
                        NavigationManager.NavigateTo($"/dashboard/languages/edit/{newLanguage.Id}");
                }
            }
            catch (Exception ex)
            {
                EditSuccessState.Success = false;
                Error = true;
                ErrorMessage = ex.Message;
                Busy = false;
            }
            finally
            {
                Busy = false;
            }
        }

        /// <summary>
        /// Back to list.
        /// </summary>
        protected void Cancel()
        {
            NavigationManager.NavigateTo("/dashboard/languages");
        }
    }
}
