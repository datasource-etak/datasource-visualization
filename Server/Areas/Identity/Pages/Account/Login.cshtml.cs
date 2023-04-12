using BlazorDatasource.Server.Infrastructure;
using BlazorDatasource.Server.Validators;
using BlazorDatasource.Shared.Identity;
using BlazorDatasource.Shared.Infrastructure;
using BlazorDatasource.Shared.Infrastructure.Models;
using BlazorDatasource.Shared.Services.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<InputModel> _validator;
        private readonly ILocalizationService _localizationService;
        private readonly DatasourceApiHttpClient _client;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
                          UserManager<ApplicationUser> userManager,
                          IValidator<InputModel> validator,
                          ILocalizationService localizationService,
                          DatasourceApiHttpClient client)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _validator = validator;
            _localizationService = localizationService;
            _client = client;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var validationResult = await _validator.ValidateAsync(Input);
            if (validationResult.IsValid)
            {
                // try to get the associated account with the username
                var applicationUser = await _userManager.FindByNameAsync(Input.Username);
                if (applicationUser is null)
                    return Page();

                // try to sign in with the password
                var loginAttempt = await _signInManager.CheckPasswordSignInAsync(applicationUser, Input.Password, lockoutOnFailure: false);
                if (loginAttempt.Succeeded)
                {
                    // try to get access token and refresh token from datasource api
                    var remoteLoginAttempt = await _client.SignInAccount(new SignInRequest()
                    {
                        Username = Input.Username,
                        Password = Input.Password
                    });

                    if (remoteLoginAttempt.Success)
                    {
                        var remoteLoginResponse = remoteLoginAttempt.Data;
                        if (remoteLoginResponse is not null)
                        {
                            // Include the access token in the properties
                            List<AuthenticationToken> tokens = new()
                            {
                                new AuthenticationToken()
                                {
                                    Name = Constants.TokenTypes.AccessToken,
                                    Value = remoteLoginResponse.AccessToken
                                },
                                new AuthenticationToken()
                                {
                                    Name = Constants.TokenTypes.RefreshToken,
                                    Value = remoteLoginResponse.RefreshToken
                                }
                            };

                            var props = new AuthenticationProperties();
                            props.StoreTokens(tokens);
                            props.IsPersistent = false;

                            // sign in with the tokens as AuthenticationProperties
                            await _signInManager.SignInAsync(applicationUser, props);
                            return LocalRedirect(returnUrl);
                        }
                    }

                    // remote login attempt failed sign in without the tokens as AuthenticationProperties
                    await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Account.Login.Invalid"));
                    return Page();
                }
            }
            else
            {
                ModelState.Clear();
                validationResult.AddToModelState(ModelState, prefix: nameof(Input));
            }

            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public class InputModel
        {
            public string Username { get; set; } = default!;

            public string Password { get; set; } = default!;
        }
    }
}
