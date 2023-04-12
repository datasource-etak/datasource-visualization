using BlazorDatasource.Server.Models.Localization;
using Microsoft.AspNetCore.Components;

namespace BlazorDatasource.Server.Pages.Dashboard.Languages
{
    public partial class LanguageRow : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        protected LanguageOverviewModel _currentLanguage = new();

        /// <summary>
        /// The Language being rendered.
        /// </summary>
        [Parameter]
        public LanguageOverviewModel CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (value is not null && !value.Equals(_currentLanguage))
                {
                    _currentLanguage = value;
                }
            }
        }

        /// <summary>
        /// Event to raise when a publish language is requested.
        /// </summary>
        [Parameter]
        public EventCallback<int> PublishLanguage { get; set; }

        /// <summary>
        /// Prevent multiple asynchronous operations at the same time.
        /// </summary>
        [Parameter]
        public bool Busy { get; set; }
    }
}
