using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NetCoreCalendar.Data;

namespace NetCoreCalendar.Configuration
{
    public class MySignInManager : SignInManager<Teacher>
    {
        public MySignInManager(UserManager<Teacher> userManager, IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<Teacher> claimsFactory, IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<Teacher>> logger, IAuthenticationSchemeProvider schemes, 
            IUserConfirmation<Teacher> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor,
                logger, schemes, confirmation)
        {
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByEmailAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
    } 
}
