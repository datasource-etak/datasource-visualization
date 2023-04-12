using BlazorDatasource.Server.Infrastructure;
using BlazorDatasource.Server.Models.Account;
using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Shared.Identity;
using BlazorDatasource.Shared.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Accounts
{
    public partial class Create : ComponentBase
    {
        [Inject]
        protected UserManager<ApplicationUser> UserManager { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected EditSuccess EditSuccessState { get; set; } = default!;

        [Inject]
        protected DatasourceApiHttpClient Client { get; set; } = default!;

        protected AccountModel? AccountModel { get; set; }

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
            AccountModel = new AccountModel();

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
                if (AccountModel is not null)
                {
                    // we first try to make the remote account
                    var remoteAccount = new SignUpRequest()
                    {
                        Username = AccountModel.Username,
                        Password = AccountModel.Password,
                        Email = AccountModel.Email,
                        FirstName = AccountModel.FirstName,
                        LastName = AccountModel.LastName
                    };

                    var remoteResult = await Client.CreateAccount(remoteAccount);
                    if (remoteResult.IsSuccessStatusCode)
                    {
                        // so the remote call worked for the account creation
                        // we now need to create the local account
                        var newAccount = new ApplicationUser()
                        {
                            UserName = AccountModel.Username,
                            Email = AccountModel.Email,
                            FirstName = AccountModel.FirstName,
                            LastName = AccountModel.LastName
                        };

                        var result = await UserManager.CreateAsync(newAccount, AccountModel.Password);
                        if (result.Succeeded)
                        {
                            EditSuccessState.Success = true;
                            EditSuccessState.SuccessMessage = T["Accounts.Added"];

                            if (!continueEditing)
                                NavigationManager.NavigateTo("/dashboard/accounts");
                            else
                                NavigationManager.NavigateTo($"/dashboard/accounts/edit/{newAccount.Id}");
                        }
                        else
                        {
                            EditSuccessState.Success = false;
                            Error = true;

                            foreach (var error in result.Errors)
                            {
                                ErrorMessage += error.Description;
                            }
                        }
                    }
                    else
                    {
                        EditSuccessState.Success = false;
                        Error = true;
                        ErrorMessage = remoteResult.ReasonPhrase ?? T["Accounts.Added.Error"];
                        Busy = false;
                    }
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
            NavigationManager.NavigateTo("/dashboard/accounts");
        }
    }
}
