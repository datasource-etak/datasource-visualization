using BlazorDatasource.Shared.Services.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorDatasource.Shared
{
    public class DatabaseStringLocalizer : IStringLocalizer
    {
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public DatabaseStringLocalizer(ILocalizationService localizationService,
                                       IWorkContext workContext)
        {
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = _localizationService.GetLocaleStringResourceByName(name, _workContext.GetWorkingLanguage().Id)?.ResourceValue;
                var result = new LocalizedString(name, value ?? name, resourceNotFound: value == null);
                return result;
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var localizedString = this[name];
                return new LocalizedString(name, string.Format(localizedString.Value, arguments), localizedString.ResourceNotFound);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            throw new KeyNotFoundException();
        }
    }
}
