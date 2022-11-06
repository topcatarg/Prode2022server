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

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns></returns>
        public async Task<bool> CreateUserAsync(NewUserData user)
        {
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
            return result > 0;
        }

        /// <summary>
        /// Check if the user is correct
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns></returns>
        public async Task<UserLogin> CheckUserAsync(UserLogin user)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            UserLogin NewUser = await db.QueryFirstOrDefaultAsync<UserLogin>(@"
select Password, Name, (case when Admin='' then 0 else coalesce(Admin,0) end) as IsAdmin, Id
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
            var Claims = GenerateClaimsForUser(User);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                //Expires = DateTime.UtcNow.AddDays(1),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<RefreshToken> GenerateRefreshToken(int UserId)
        {
            RefreshToken refreshToken = new RefreshToken();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(1);
            //Store the token on the database
            using SqliteConnection db = database.SimpleDbConnection();
            await db.ExecuteAsync(@"
insert into RefreshTokens (UserId,Token,ExpiryDate) 
values (@UserId, @Token, @Expiry)
            ", new
            {
                UserId,
                Token = refreshToken.Token,
                Expiry = refreshToken.ExpiryDate
            });
            return refreshToken;
        }

        public UserLogin GetUserByAccessToken(string accessToken)
        {
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
                ClaimsPrincipal? principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken? jwtSecurityToken = (JwtSecurityToken)securityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    //If found get claims and return them.
                    return new UserLogin()
                    {
                        Id = principle.GetClaim<int>(ClaimType.Id),
                        Email = principle.GetClaim(ClaimType.Mail),
                        Name = principle.GetClaim(ClaimType.Name),
                        IsAdmin = principle.GetClaim<int>(ClaimType.IsAdmin)==1?true:false,
                        LoggedIn = true
                    };
                }
            }
            catch (SecurityTokenExpiredException)
            {
                //Try to generate a new token form RefreshedOne
                return new UserLogin()
                {
                    IsExpired = true
                };
            }
            catch (Exception)
            {
                return new UserLogin();
            }

            return new UserLogin();
        }

        public  async Task<UserLogin> GetUserByRefreshTokenAsync(string Token)
        {
            UserLogin newUser = new();
            using SqliteConnection db = database.SimpleDbConnection();
            newUser = await db.QueryFirstOrDefaultAsync<UserLogin>(@"
select Password, Name, Admin as IsAdmin, Id, Email
from users
where Id = (
        select UserId
        from RefreshTokens
        where Token = @Token
            and 
            ExpiryDate >= @date
        Limit 1)
",
            new
            {
                Token,
                date = DateTime.UtcNow
            });
            if (newUser == null)
            {
                newUser = new();
            }
            if (newUser.Email != null)
            {
                newUser.LoggedIn = true;
            }
            return newUser;
        }

        public List<Claim> GenerateClaimsForUser(UserLogin User)
        {
            if (User != null)
            {
                return new List<Claim>()
                {
                    new Claim(ClaimType.Mail,User.Email!),
                    new Claim(ClaimType.Id,User.Id.ToString()),
                    new Claim(ClaimType.Name, User.Name!),
                    new Claim(ClaimType.IsAdmin, User.IsAdmin?"1":"0")
                };
            }
            return new List<Claim>();
        }

       

    }
}


