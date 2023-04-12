using BlazorDatasource.Server.Areas.Identity.Pages.Account;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorDatasource.Server.Validators.Account
{
    public class LoginInputModelValidator : AbstractValidator<LoginModel.InputModel>
    {
        public LoginInputModelValidator(IStringLocalizer stringLocalizer)
        {
            RuleFor(property => property.Username).NotEmpty()
                                                  .WithMessage(x => stringLocalizer["Account.Login.Fields.Username.Required"]);

            RuleFor(property => property.Password).NotEmpty()
                                                  .WithMessage(x => stringLocalizer["Account.Login.Fields.Password.Required"]);
        }
    }
}
