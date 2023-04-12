using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace BlazorDatasource.Api
{
    /// <summary>
    /// ClaimsPrincipal extensions
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// PreferredUserName: "preferred_username".
        /// </summary>
        private const string PreferredUserName = "preferred_username";

        /// <summary>
        /// Gets the name using the JwtRegisteredClaimNames.Name claim
        /// </summary>
        /// <param name="user">The System.Security.Claims.ClaimsPrincipal</param>
        /// <returns>The value of the first instance, or null if the claim is not present</returns>
        public static string GetName(this ClaimsPrincipal user) => user.FindFirstValue(JwtRegisteredClaimNames.Name);

        /// <summary>
        /// Gets the email using the JwtRegisteredClaimNames.Email claim
        /// </summary>
        /// <param name="user">The System.Security.Claims.ClaimsPrincipal</param>
        /// <returns>The value of the first instance, or null if the claim is not present</returns>
        public static string GetEmail(this ClaimsPrincipal user) => user.FindFirstValue(JwtRegisteredClaimNames.Email);

        /// <summary>
        /// Gets the identifier using the JwtRegisteredClaimNames.Sub claim
        /// </summary>
        /// <param name="user">The System.Security.Claims.ClaimsPrincipal</param>
        /// <returns>The value of the first instance, or null if the claim is not present</returns>
        public static string GetId(this ClaimsPrincipal user) => user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        /// <summary>
        /// Gets the display name using the PreferredUserName claim
        /// </summary>
        /// <param name="user">The System.Security.Claims.ClaimsPrincipal</param>
        /// <returns>The value of the first instance, or null if the claim is not present</returns>
        public static string GetDisplayName(this ClaimsPrincipal user) => user.FindFirstValue(PreferredUserName);
    }
}
