namespace BlazorDatasource.Server.Models.Localization
{
    /// <summary>
    /// Represents a language model for list
    /// </summary>
    public partial record LanguageOverviewModel : BaseEntityModel
    {
        public string Name { get; set; } = default!;

        public string FlagImageFileName { get; set; } = default!;

        public bool Published { get; set; }

        public string UniqueSeoCode { get; set; } = default!;

        public string LanguageCulture { get; set; } = default!;
    }
}
