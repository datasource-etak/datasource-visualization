namespace BlazorDatasource.Server.Models.Account
{
    /// <summary>
    /// Represents an account model
    /// </summary>
    public partial record AccountModel : BaseEntityModel
    {
        public string? AccountId { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        #region Additional properties

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        #endregion
    }
}
