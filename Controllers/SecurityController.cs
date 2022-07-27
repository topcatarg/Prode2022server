using Microsoft.AspNetCore.Mvc;
using Prode2022Server.Services;
using Prode2022Server.Models;
using Prode2022Server.Helpers;
using Microsoft.IdentityModel.Tokens;


namespace Prode2022Server.Controllers
{
    [Route("Security")]
    [ApiController]
    public class SecurityController : Controller
    {

        SecurityServices securityServices;


        public SecurityController(SecurityServices ses
            )
        {
            securityServices = ses;

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
                //crear la clave jwt
                user.AccessToken = securityServices.GenerateAccessToken(user);
                var RefreshToken = await securityServices.GenerateRefreshToken(user.Id);
                user.RefreshToken = RefreshToken.Token;
            }
            return user;
        }

        
    }
}
