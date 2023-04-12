using BlazorDatasource.Server.Models.Account;
using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Shared.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Accounts
{
    public partial class Edit : ComponentBase
    {
        [Inject]
        protected UserManager<ApplicationUser> UserManager { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected EditSuccess EditSuccessState { get; set; } = default!;

        /// <summary>
        /// Id of Account to edit
        /// </summary>
        [Parameter]
        public string AccountId { get; set; } = default!;

        /// <summary>
        /// Account being edited
        /// </summary>
        protected AccountModel? AccountModel { get; set; }

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
                var account = await UserManager.FindByIdAsync(AccountId);
                if (account is null)
                {
                    return;
                }

                AccountModel = new AccountModel()
                {
                    AccountId = account.Id,
                    Username = account.UserName,
                    Email = account.Email,
                    FirstName = account.FirstName,
                    LastName = account.LastName
                };
            }
            finally
            {
                Busy = false;
            }

            await base.OnInitializedAsync();
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
