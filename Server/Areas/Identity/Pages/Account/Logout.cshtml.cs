using BlazorDatasource.Shared.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Areas.Identity.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager,
                           UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string returnUrl = Url.Content("~/");

            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
                var identityUser = await _userManager.FindByNameAsync(User.Identity?.Name);
                await _userManager.UpdateSecurityStampAsync(identityUser);
            }

            return LocalRedirect(returnUrl);
        }
    }
}
