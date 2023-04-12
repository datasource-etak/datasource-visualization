using BlazorDatasource.Server.Models.Account;
using BlazorDatasource.Server.Pages.Dashboard.Languages;
using BlazorDatasource.Server.Shared;
using BlazorDatasource.Shared.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Accounts
{
    public partial class AccountForm : ComponentBase
    {
        [Inject]
        protected IValidator<AccountModel> Validator { get; set; } = default!;

        [Inject]
        protected UserManager<ApplicationUser> UserManager { get; set; } = default!;

        protected FluentValidationSummary? FluentValidation;

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
        /// The Account to upsert.
        /// </summary>
        [Parameter]
        public AccountModel? AccountModel { get; set; }

        /// <summary>
        /// Prevent multiple asynchronous operations at the same time.
        /// </summary>
        [Parameter]
        public bool Busy { get; set; }

        /// <summary>
        /// Mode.
        /// </summary>
        protected string Mode => IsAdd ? T["Accounts.Create"] : T["Accounts.Edit"];

        /// <summary>
        /// Do we need to navigate to the list or the edit page
        /// </summary>
        protected bool ContinueEditing { get; set; }

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
            if (AccountModel is null)
                return;

            FluentValidation?.ClearErrors();

            var validationResults = await Validator.ValidateAsync(AccountModel);
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
}
