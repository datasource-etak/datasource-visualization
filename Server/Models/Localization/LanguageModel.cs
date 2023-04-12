namespace BlazorDatasource.Server.Models.Localization
{
    /// <summary>
    /// Represents a language model
    /// </summary>
    public partial record LanguageModel : BaseEntityModel
    {
        #region Properties

        public string? Name { get; set; }

        public string? LanguageCulture { get; set; }

        public string? UniqueSeoCode { get; set; }

        //flags
        public string? FlagImageFileName { get; set; }

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }

        #endregion
    }
}
