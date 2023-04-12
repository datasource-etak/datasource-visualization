namespace BlazorDatasource.Server.Models.Localization
{
    /// <summary>
    /// Represents a language model for selection
    /// </summary>
    public partial record LanguageSelectionModel : BaseEntityModel
    {
        public string Name { get; set; } = default!;

        public string FlagImageFileName { get; set; } = default!;

        public bool Selected = false;
    }
}
