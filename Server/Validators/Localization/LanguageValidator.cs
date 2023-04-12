using BlazorDatasource.Server.Models.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace BlazorDatasource.Server.Validators.Localization
{
    public partial class LanguageValidator : AbstractValidator<LanguageModel>
    {
        public LanguageValidator(IStringLocalizer stringLocalizer)
        {
            RuleFor(property => property.Name).NotEmpty().WithMessage(x => stringLocalizer["Languages.Fields.Name.Required"]);
            RuleFor(property => property.LanguageCulture)
                .Must(property =>
                {
                    if (property is null)
                    {
                        return false;
                    }
                    try
                    {
                        //let's try to create a CultureInfo object
                        //if "DisplayLocale" is wrong, then exception will be thrown
                        var unused = new CultureInfo(property);
                        return true;

                    }
                    catch
                    {
                        return false;
                    }
                })
                .WithMessage(x => stringLocalizer["Languages.Fields.LanguageCulture.InvalidLanguageCulture"]);

            RuleFor(property => property.UniqueSeoCode).NotEmpty().WithMessage(x => stringLocalizer["Languages.Fields.UniqueSeoCode.Required"]);
            RuleFor(property => property.UniqueSeoCode).Length(2).WithMessage(x => string.Format(stringLocalizer["Languages.Fields.UniqueSeoCode.Length"], "{MinLength}"));
        }
    }
}
