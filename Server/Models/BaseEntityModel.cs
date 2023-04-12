namespace BlazorDatasource.Server.Models
{
    /// <summary>
    /// Represents base entity model
    /// </summary>
    public partial record BaseEntityModel : BaseModel
    {
        /// <summary>
        /// Gets or sets model identifier
        /// </summary>
        public virtual int Id { get; set; }
    }
}
