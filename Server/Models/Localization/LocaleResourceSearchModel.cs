namespace BlazorDatasource.Server.Models.Localization
{
    /// <summary>
    /// Represents a locale resource search model
    /// </summary>
    public partial record LocaleResourceSearchModel : BaseSearchModel
    {
        #region Ctor

        public LocaleResourceSearchModel()
        {
            AddResourceString = new LocaleResourceModel();
        }

        #endregion

        #region Properties

        public int LanguageId { get; set; }

        public string SearchResourceName { get; set; } = default!;

        public string SearchResourceValue { get; set; } = default!;

        public LocaleResourceModel AddResourceString { get; set; }

        #endregion
    }
}
