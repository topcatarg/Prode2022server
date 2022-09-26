namespace Prode2022Server.Services;

using Prode2022Server.Models.UserData;
using Dapper;
using Microsoft.Data.Sqlite;

public class UserDataServices
{
    private readonly DbService database;

    public UserDataServices(DbService dbService)
    {
        database = dbService;
    }

    public async Task<UserLoginData> GetUserDataAsync(int UserId)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var result =  await db.QueryFirstAsync<UserLoginData>(@"
select Id, Name, Email 
from Users
where Id = @UserId",
            new
            {
                UserId
            });
        return result;
    }

    public async Task<bool> ChangeUserNameAsync(UserName userName)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var result = await db.ExecuteAsync(@"
update Users 
set Name = @Name
where Id = @UserId",
            new
            {
                Name = userName.Name,
                UserId = userName.Id
            });
        return result > 0;
    }

    public async Task<bool> ChangeUserEmailAsync(UserEmail userEmail)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var result = await db.ExecuteAsync(@"
update Users 
set Email = @Email
where Id = @UserId",
            new
            {
                Email = userEmail.Email,
                UserId = userEmail.Id
            });
        return result > 0;
    }

    public async Task<bool> ChangeUserPasswordAsync(UserPassword userPassword)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        string PasswordHash = BCrypt.Net.BCrypt.HashPassword(userPassword.Password);
        var result = await db.ExecuteAsync(@"
update Users 
set Password = @Password
where Id = @UserId",
            new
            {
                Password = PasswordHash,
                UserId = userPassword.Id
            });
        if (result > 0)
        {
            result = await db.ExecuteAsync(@"
Delete from RefreshTokens
where UserId = @UserId",
                new
                {
                    UserId = userPassword.Id
                });
        }
        return result > 0;
    }

}