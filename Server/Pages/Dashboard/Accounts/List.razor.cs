using BlazorDatasource.Server.Models.Account;
using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Server.Models.Extensions;
using BlazorDatasource.Shared.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Pages.Dashboard.Accounts
{
    public partial class List : ComponentBase
    {
        [Inject]
        protected UserManager<ApplicationUser> UserManager { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected EditSuccess EditSuccessState { get; set; } = default!;

        /// <summary>
        /// Search user names
        /// </summary>
        public string SearchUsername { get; set; } = default!;

        /// <summary>
        /// Search emails
        /// </summary>
        public string SearchEmail { get; set; } = default!;

        /// <summary>
        /// Gets or sets a page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets a page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of available page sizes
        /// </summary>
        public string? AvailablePageSizes { get; set; }

        /// <summary>
        /// Accounts list
        /// </summary>
        protected AccountListModel? Accounts { get; set; }

        /// <summary>
        /// Avoid multiple concurrent requests when loading.
        /// </summary>
        protected bool Loading { get; set; }

        /// <summary>
        /// Avoid multiple concurrent requests when loading.
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
        /// Component initialization
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task OnInitializedAsync()
        {
            Loading = true;

            // Paging
            Page = 1;
            PageSize = PagingDefaults.DefaultGridPageSize;
            AvailablePageSizes = PagingDefaults.GridPageSizes;

            Loading = false;

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Triggered for any paging update.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task OnParametersSetAsync()
        {
            ReloadAsync();
            await base.OnParametersSetAsync();
        }

        /// <summary>
        /// Reload page
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected void ReloadAsync()
        {
            if (Loading || Page < 1)
            {
                return;
            }

            if (Busy)
            {
                return;
            }

            Loading = true;

            //get accounts
            var accounts = UserManager.Users
                .OrderBy(account => account.UserName)
                .AsQueryable();

            var pagedAccounts = accounts.ToPagedList(Page - 1, PageSize);

            //filter account
            if (!string.IsNullOrEmpty(SearchUsername))
                accounts = accounts.Where(a => a.NormalizedUserName.Contains(UserManager.NormalizeName(SearchUsername)));
            if (!string.IsNullOrEmpty(SearchEmail))
                accounts = accounts.Where(a => a.NormalizedEmail.Contains(UserManager.NormalizeEmail(SearchEmail)));

            //prepare list model
            Accounts = new AccountListModel().PrepareToGrid(pagedAccounts, () =>
            {
                //fill in model values from the entity
                return accounts.Select(account => new AccountModel
                {
                    AccountId = account.Id,
                    Username = account.UserName,
                    Email = account.Email,
                    FirstName = account.FirstName,
                    LastName = account.LastName
                });
            });

            Loading = false;
        }

        /// <summary>
        /// OnPageSizeChanged event
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected void OnPageSizeChanged(ChangeEventArgs args)
        {
            if (args.Value is not null)
            {
                Page = 1;
                PageSize = Convert.ToInt32(args.Value);
                ReloadAsync();
            }
        }

        /// <summary>
        /// OnSearchUsernameChanged event
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected void OnSearchUsernameChanged(ChangeEventArgs args)
        {
            if (args.Value is not null)
            {
                Page = 1;
                SearchUsername = (string)args.Value;
                ReloadAsync();
            }
        }

        /// <summary>
        /// OnSearchEmailChanged event
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected void OnSearchEmailChanged(ChangeEventArgs args)
        {
            if (args.Value is not null)
            {
                Page = 1;
                SearchEmail = (string)args.Value;
                ReloadAsync();
            }
        }
    }
}
