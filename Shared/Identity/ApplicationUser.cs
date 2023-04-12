using Microsoft.AspNetCore.Identity;

namespace BlazorDatasource.Shared.Identity
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }
    }
}
