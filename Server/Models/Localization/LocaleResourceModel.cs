namespace BlazorDatasource.Server.Models.Localization
{
    /// <summary>
    /// Represents a locale resource model
    /// </summary>
    public partial record LocaleResourceModel : BaseEntityModel
    {
        #region Properties

        public string? ResourceName { get; set; }

        public string? ResourceValue { get; set; }

        public int LanguageId { get; set; }

        #endregion
    }
}
