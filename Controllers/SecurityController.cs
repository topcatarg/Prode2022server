using Microsoft.AspNetCore.Mvc;
using Prode2022Server.Services;
using System.Linq;
using System.Collections.Immutable;
using Prode2022Server.Models;
using Prode2022Server.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Prode2022Server.Controllers
{
    [Route("Security")]
    [ApiController]
    public class SecurityController : Controller
    {

        SecurityServices securityServices;
        IHttpContextAccessor HttpContextAccessor;

        public SecurityController(SecurityServices ses,
            IHttpContextAccessor httpContextAccessor)
        {
            securityServices = ses;
            HttpContextAccessor = httpContextAccessor;
        }

        [Route("CreateUser")]
        [HttpPost]
        public async Task<bool> CreateUser(NewUserData user)
        {
            if(
                user.UserName.IsNullOrEmpty()
                || user.Email.IsNullOrEmpty()
                || user.Password.IsNullOrEmpty())
            {
                return false;
            }
            return await securityServices.CreateUser(user);
        }

        [Route("LoginUser")]
        [HttpPost]
        public async Task<UserLogin> LoginUser(UserLogin user)
        {
            if(
                user.Email.IsNullOrEmpty()
                || user.Password.IsNullOrEmpty())
            {
                user.LoggedIn = false;
                return user;
            }
            user = await securityServices.CheckUser(user);
            if (user.LoggedIn)
            {
                //create a session for the user.
                var claims = new List<Claim>();
                claims.Add(new (ClaimTypes.Name,user.Name!));
                claims.Add(new (ClaimTypes.Email,user.Email!));
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var v = await HttpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );
                
            }
            bool isok = HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            return user;
        }
    }
}
