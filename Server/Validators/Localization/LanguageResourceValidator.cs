using BlazorDatasource.Server.Models.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorDatasource.Server.Validators.Localization
{
    public partial class LanguageResourceValidator : AbstractValidator<LocaleResourceModel>
    {
        public LanguageResourceValidator(IStringLocalizer stringLocalizer)
        {
            RuleFor(property => property.ResourceName).NotEmpty()
                                                      .WithMessage(x => stringLocalizer["Languages.Resources.Fields.Name.Required"]);

            RuleFor(property => property.ResourceValue).NotEmpty()
                                                       .WithMessage(x => stringLocalizer["Languages.Resources.Fields.Value.Required"]);
        }
    }
}
