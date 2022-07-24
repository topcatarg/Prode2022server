using Prode2022Server.Models;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Linq;
using BCrypt.Net;
using Prode2022Server.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

namespace Prode2022Server.Services
{
    public class SecurityServices
    {
        private readonly DbService database;
        private readonly SettingHelpers settings;

        public SecurityServices(DbService dbService, SettingHelpers settings)
        {
            database = dbService;
            this.settings = settings;
        }

        public async Task<bool> CreateUser(NewUserData user)
        {
            //lets encrypt the password first
            //get a new salt
            //var salt = RandomNumberGenerator.GetBytes(32).ToString();
            //var salt2 = BCrypt.Net.BCrypt.GenerateSalt();
            //string PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, salt2);
            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            using SqliteConnection db = database.SimpleDbConnection();
            int result = await db.ExecuteAsync(@"
insert into users (name, email, password, salt, validated) 
values (@name, @email, @password, @salt, @validated)",
                new
                {
                    name = user.UserName,
                    email = user.Email,
                    password = PasswordHash,
                    salt = "",
                    validated = false
                }
            );
            return result>0;
        }

        public async Task<UserLogin> CheckUser(UserLogin user)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            UserLogin NewUser = await db.QueryFirstOrDefaultAsync<UserLogin>(@"
select Password, Name, Admin as IsAdmin, Id
from users
where email = @email",
                new
                {
                    email = user.Email
                });
            NewUser.LoggedIn = BCrypt.Net.BCrypt.Verify(user.Password, NewUser.Password);
            NewUser.Email = user.Email;
            NewUser.Password = "";
            return NewUser;
        }
        
        public string GenerateAccessToken(UserLogin User)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(settings.JwtSettings.SecretKey ?? "DefaultKeyIfNotFound");
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

        public async Task<UserLogin> GetUserByAccessTokenAsync(string accessToken)
        {
            //Serialize token
            string serializedRefreshRequest = JsonSerializer.Serialize(accessToken);
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(settings.JwtSettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(serializedRefreshRequest, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userId = principle.FindFirst(ClaimType.Mail)?.Value;
                    if (userId != null)
                    {
                        return await GetUserByRefreshToken(userId);
                    }


                }
            }
            catch (Exception)
            {
                return new UserLogin();
            }

            return new UserLogin();
        }

        private async Task<UserLogin> GetUserByRefreshToken(string Token)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            UserLogin NewUser = await db.QueryFirstOrDefaultAsync<UserLogin>(@"
select Password, Name, Admin as IsAdmin, Id, email as Email
from users
where email = @email",
                new
                {
                    email = Token
                });
            NewUser.LoggedIn = true;
            return NewUser;
        }
    }
}
