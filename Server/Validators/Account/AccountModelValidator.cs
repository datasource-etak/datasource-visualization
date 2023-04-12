using BlazorDatasource.Server.Models.Account;
using BlazorDatasource.Shared;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorDatasource.Server.Validators.Account
{
    public class AccountModelValidator : AbstractValidator<AccountModel>
    {
        public AccountModelValidator(IStringLocalizer stringLocalizer)
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage(x => stringLocalizer["Accounts.Fields.Username.Required"]);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(x => stringLocalizer["Accounts.Fields.Email.Required"])
                .Must(x => CommonHelper.IsValidEmail(x))
                .WithMessage(x => stringLocalizer["Accounts.Fields.Email.WrongFormat"]);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(x => stringLocalizer["Accounts.Fields.FirstName.Required"]);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(x => stringLocalizer["Accounts.Fields.LastName.Required"]);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(x => stringLocalizer["Accounts.Fields.Password.Required"])
                .MinimumLength(6)
                .WithMessage(x => stringLocalizer["Accounts.Fields.Password.MinimumLength"]);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(x => stringLocalizer["Accounts.Fields.ConfirmPassword.Required"])
                .Equal(x => x.Password)
                .WithMessage(x => stringLocalizer["Accounts.Fields.ConfirmPassword.NoMatch"]);
        }
    }
}
