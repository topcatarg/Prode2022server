using Prode2022Server.Models;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Linq;
using BCrypt.Net;

namespace Prode2022Server.Services
{
    public class SecurityServices
    {
        private readonly DbService database;

        public SecurityServices(DbService dbService)
        {
            database = dbService;
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
select Password, Name, Admin as IsAdmin
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
    }
}
