using BlazorDatasource.Server.Models.Localization;
using BlazorDatasource.Server.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Languages
{
    public partial class LanguageForm : ComponentBase
    {
        [Inject]
        protected IValidator<LanguageModel> Validator { get; set; } = default!;

        [Inject]
        protected IWebHostEnvironment Environment { get; set; } = default!;

        protected FluentValidationSummary? FluentValidation;

        protected IList<SelectListItem> AvailableCultures { get; set; } = new List<SelectListItem>();

        protected IList<SelectListItem> AvailableFlagFileNames { get; set; } = new List<SelectListItem>();

        #region Const

        private const string FLAGS_PATH = @"flags";

        #endregion

        /// <summary>
        /// Let parent handle result of validation.
        /// </summary>
        [Parameter]
        public EventCallback<HandleSubmitEventCallbackArgs> ValidationResult { get; set; }

        /// <summary>
        /// Let parent handle what to do on cancel.
        /// </summary>
        [Parameter]
        public EventCallback CancelRequest { get; set; }

        /// <summary>
        /// <c>True</c> if add mode.
        /// </summary>
        [Parameter]
        public bool IsAdd { get; set; }

        /// <summary>
        /// The Language to upsert.
        /// </summary>
        [Parameter]
        public LanguageModel? LanguageModel { get; set; }

        /// <summary>
        /// Prevent multiple asynchronous operations at the same time.
        /// </summary>
        [Parameter]
        public bool Busy { get; set; }

        /// <summary>
        /// Mode.
        /// </summary>
        protected string Mode => IsAdd ? T["Languages.Create"] : T["Languages.Edit"];

        /// <summary>
        /// Do we need to navigate to the list or the edit page
        /// </summary>
        protected bool ContinueEditing { get; set; }

        /// <summary>
        /// Component initialization
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override Task OnParametersSetAsync()
        {
            AvailableCultures = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures)
                .OrderBy(x => x.EnglishName)
                .Select(x => new SelectListItem
                {
                    Value = x.IetfLanguageTag,
                    Text = $"{x.EnglishName}. {x.IetfLanguageTag}"
                }).ToList();

            using var fileProvider = new PhysicalFileProvider(File.Exists(Environment.ContentRootPath) ?
                                                              Path.GetDirectoryName(Environment.ContentRootPath) :
                                                              Environment.ContentRootPath);
            var webRootPath = File.Exists(Environment.WebRootPath) ?
                Path.GetDirectoryName(Environment.WebRootPath) :
                Environment.WebRootPath;
            var paths = new List<string>
            {
                webRootPath ?? string.Empty,
                FLAGS_PATH
            };
            var path = Path.Combine(paths.SelectMany(p => p.Split('\\', '/')).ToArray());
            var flagNames = Directory.EnumerateFiles(path, "*.png", SearchOption.TopDirectoryOnly)
               .Select(path => Path.GetFileName(path))
               .ToList();

            AvailableFlagFileNames = flagNames.Select(flagName => new SelectListItem
            {
                Text = flagName,
                Value = flagName
            }).ToList();

            if (LanguageModel is not null)
            {
                if (LanguageModel.LanguageCulture is null)
                    LanguageModel.LanguageCulture = AvailableCultures.FirstOrDefault()?.Value;
                if (LanguageModel.FlagImageFileName is null)
                    LanguageModel.FlagImageFileName = AvailableFlagFileNames.FirstOrDefault()?.Value;
            }

            return base.OnParametersSetAsync();
        }

        /// <summary>
        /// Ask to cancel.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected Task CancelAsync()
        {
            return CancelRequest.InvokeAsync(null);
        }

        /// <summary>
        /// Handle form submission request.
        /// </summary>
        /// <param name="isValid"><c>True</c> when field validation passed.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task HandleSubmitAsync()
        {
            if (LanguageModel is null)
                return;

            FluentValidation?.ClearErrors();

            var validationResults = await Validator.ValidateAsync(LanguageModel);
            if (validationResults.Errors.Any())
            {
                var errors = new Dictionary<string, string>();
                foreach (var error in validationResults.Errors)
                {
                    errors.TryAdd(error.PropertyName, error.ErrorMessage);
                }
                FluentValidation?.DisplayErrors(errors);
            }

            var args = new HandleSubmitEventCallbackArgs { Success = validationResults.IsValid, ContinueEditing = ContinueEditing };
            await ValidationResult.InvokeAsync(args);
        }
    }

    public record HandleSubmitEventCallbackArgs
    {
        /// <summary>
        /// <c>True</c> if validation succeeded
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// <c>True</c> if we need to navigate to the edit page.<c>False</c> if we need to navigate to the list
        /// </summary>
        public bool ContinueEditing { get; set; }
    }
}
