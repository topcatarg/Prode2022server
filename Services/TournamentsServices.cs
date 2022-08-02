namespace Prode2022Server.Services;

using Dapper;
using Microsoft.Data.Sqlite;
using Prode2022Server.Models;
public class TournamentsServices
{

    private readonly DbService database;

    public TournamentsServices(DbService dbService)
    {
        database = dbService;
    }

    /// <summary>
    /// Get all the tournaments administrates by an user
    /// </summary>
    /// <param name="User"></param>
    /// <returns>a list of tournaments administrates by this user</returns>
    public async Task GetUserAdministrateTournaments(int User)
    {

    }

    /// <summary>
    /// Create a tournament. It verify first that the name is unique
    /// </summary>
    /// <param name="tournaments"></param>
    /// <returns>An empty string is everything is correct. An error message in other case</returns>
    public async Task<string> CreateTournaments(Tournament tournament)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        int result = await db.ExecuteAsync(@"
Insert into tournaments (Name, Password, AdministratorId)
values
    (@Name, @Password, @AdministratorId)
",
            new
            {
                tournament.Name,
                tournament.Password,
                tournament.AdministratorId
            });
        if (result == 0)
        {
            return "Ya existe un torneo con ese nombre";
        }
        return "";
    }
}