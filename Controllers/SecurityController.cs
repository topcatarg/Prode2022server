using Microsoft.AspNetCore.Mvc;
using Prode2022Server.Services;
using System.Linq;
using System.Collections.Immutable;
using Prode2022Server.Models;
using Prode2022Server.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Prode2022Server.Controllers
{
    [Route("Security")]
    [ApiController]
    public class SecurityController : Controller
    {

        SecurityServices securityServices;
        private readonly JWTSettings _jwtsettings;

        public SecurityController(SecurityServices ses,
            JWTSettings jwtsettings)
        {
            securityServices = ses;
            _jwtsettings = jwtsettings;
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
                user.AccessToken = GenerateAccessToken(user);

            }
            return user;
        }

        private string GenerateAccessToken(UserLogin User)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey??"DefaultKey");
            var Claims = new List<Claim>()
            {
                new Claim(ClaimType.Mail,User.Email!),
                new Claim(ClaimType.Id,User.Id.ToString())
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
