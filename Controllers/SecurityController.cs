using Microsoft.AspNetCore.Mvc;
using Prode2022Server.Services;
using System.Linq;
using System.Collections.Immutable;
using Prode2022Server.Models;
using Prode2022Server.Helpers;

namespace Prode2022Server.Controllers
{
    [Route("Security")]
    [ApiController]
    public class SecurityController : Controller
    {

        SecurityServices securityServices;

        public SecurityController(SecurityServices ses)
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
            return await securityServices.CheckUser(user);
        }
    }
}
