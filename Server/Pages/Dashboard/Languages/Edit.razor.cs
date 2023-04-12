using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Server.Models.Localization;
using BlazorDatasource.Server.Shared;
using BlazorDatasource.Shared.Services.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Languages
{
    public partial class Edit : ComponentBase
    {
        [Inject]
        protected ILanguageService LanguageService { get; set; } = default!;

        [Inject]
        protected ILocalizationService LocalizationService { get; set; } = default!;

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected EditSuccess EditSuccessState { get; set; } = default!;

        /// <summary>
        /// Id of Language to edit
        /// </summary>
        [Parameter]
        public int LanguageId { get; set; }

        /// <summary>
        /// Language being edited
        /// </summary>
        protected LanguageModel? LanguageModel { get; set; }

        /// <summary>
        /// Avoid concurrent requests
        /// </summary>
        protected bool Busy;

        /// <summary>
        /// An error occurred in the update
        /// </summary>
        protected bool Error;

        /// <summary>
        /// Error message
        /// </summary>
        protected string ErrorMessage = string.Empty;

        /// <summary>
        /// Start it up
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task OnInitializedAsync()
        {
            Busy = true;

            try
            {
                var language = await LanguageService.GetLanguageByIdAsync(LanguageId);
                if (language is null)
                {
                    return;
                }

                LanguageModel = new LanguageModel()
                {
                    Id = language.Id,
                    Name = language.Name,
                    LanguageCulture = language.LanguageCulture,
                    UniqueSeoCode = language.UniqueSeoCode,
                    FlagImageFileName = language.FlagImageFileName,
                    DisplayOrder = language.DisplayOrder,
                    Published = language.Published
                };
            }
            finally
            {
                Busy = false;
            }

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
                // still need to edit model
                Error = false;
                return;
            }

            Busy = true;

            try
            {
                var language = await LanguageService.GetLanguageByIdAsync(LanguageId);
                if (language is null)
                    return;

                language.Name = LanguageModel?.Name;
                language.LanguageCulture = LanguageModel?.LanguageCulture;
                language.UniqueSeoCode = LanguageModel?.UniqueSeoCode;
                language.FlagImageFileName = LanguageModel?.FlagImageFileName;
                language.DisplayOrder = LanguageModel?.DisplayOrder ?? 0;
                language.Published = LanguageModel?.Published ?? false;

                await LanguageService.UpdateLanguageAsync(language);
                EditSuccessState.Success = true;
                EditSuccessState.SuccessMessage = T["Languages.Updated"];

                if (!continueEditing)
                    NavigationManager.NavigateTo("/dashboard/languages");
                else
                    NavigationManager.NavigateTo($"/dashboard/languages/edit/{language?.Id}");
            }
            catch (Exception ex)
            {
                EditSuccessState.Success = false;
                // unknown exception
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

        /// <summary>
        /// Confirm deletion
        /// </summary>
        /// <param name="result">True when user confirmed</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ConfirmAsync(bool result)
        {
            if (result)
            {
                await DeleteAsync();
            }
            else
            {
                ShowConfirmation = false;
            }
        }

        /// <summary>
        /// Set to true when delete is requested.
        /// </summary>
        protected bool ShowConfirmation = false;

        /// <summary>
        /// Deletes the contact.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task DeleteAsync()
        {
            if (Busy)
            {
                return; // avoid concurrent requests
            }

            Busy = true;

            //ensure we have at least one published language
            var allLanguages = await LanguageService.GetAllLanguagesAsync();
            if (allLanguages.Count == 1 && allLanguages[0].Id == LanguageModel?.Id)
            {
                EditSuccessState.Success = false;
                Error = true;
                ErrorMessage = T["Languages.PublishedLanguageRequired"];
                Busy = false;
                ShowConfirmation = false;
                return;
            }

            try
            {
                var language = await LanguageService.GetLanguageByIdAsync(LanguageId);
                if (language is null)
                    return;

                await LanguageService.DeleteLanguageAsync(language);
                EditSuccessState.Success = true;
                EditSuccessState.SuccessMessage = T["Languages.Deleted"];

                NavigationManager.NavigateTo("/dashboard/languages");
            }
            catch (Exception ex)
            {
                EditSuccessState.Success = false;
                // unknown exception
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
        /// Export resources to xml
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ExportXml()
        {
            //try to get a language with the specified id
            var language = await LanguageService.GetLanguageByIdAsync(LanguageId);
            if (language is null)
                return;

            if (Busy)
            {
                return; // avoid concurrent requests
            }

            Busy = true;

            try
            {
                var xml = await LocalizationService.ExportResourcesToXmlAsync(language);
                var fileName = $"language_pack_{language.UniqueSeoCode}.xml";

                var buffer = Encoding.UTF8.GetBytes(xml);
                var stream = new MemoryStream(buffer);

                using var streamRef = new DotNetStreamReference(stream: stream);
                await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
            }
            catch (Exception ex)
            {
                EditSuccessState.Success = false;
                // unknown exception
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
        /// Set to true when file upload is requested.
        /// </summary>
        protected bool ShowFileUploadDialog = false;

        /// <summary>
        /// Import resources from xml
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ImportXml(bool result, List<UploadResult> uploadFiles)
        {
            if (result)
            {
                if (Busy)
                {
                    return; // avoid concurrent requests
                }

                Busy = true;

                var language = await LanguageService.GetLanguageByIdAsync(LanguageId);
                if (language is null)
                    return;

                try
                {
                    if (uploadFiles != null && uploadFiles.Count > 0)
                    {
                        var temporaryFilePath = uploadFiles[0]?.StoredFileName;
                        if (temporaryFilePath is null)
                            return;

                        if (File.Exists(temporaryFilePath))
                        {
                            using var sr = new StreamReader(File.OpenRead(temporaryFilePath), Encoding.UTF8);
                            await LocalizationService.ImportResourcesFromXmlAsync(language, sr);
                        }
                    }
                    else
                    {
                        EditSuccessState.Success = false;
                        Error = true;
                        ErrorMessage = T["Common.UploadFile"];
                        Busy = false;
                        NavigationManager.NavigateTo($"/dashboard/languages/edit/{language.Id}");
                    }

                    EditSuccessState.Success = true;
                    EditSuccessState.SuccessMessage = T["Languages.Resources.Imported"];

                    NavigationManager.NavigateTo($"/dashboard/languages/resources/{language.Id}");
                }
                catch (Exception ex)
                {
                    EditSuccessState.Success = false;
                    // unknown exception
                    Error = true;
                    ErrorMessage = ex.Message;
                    Busy = false;
                }
                finally
                {
                    Busy = false;
                }
            }
            else
            {
                ShowFileUploadDialog = false;
            }
        }
    }
}
