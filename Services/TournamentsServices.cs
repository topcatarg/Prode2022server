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
    public async Task<List<Tournament>> GetUserAdministrateTournaments(int User)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var v = await db.QueryAsync<Tournament>(@"
select * 
from tournaments
where AdministratorId = @Id",
        new
        {
            Id = User,
        });
        return v.ToList();
    }

    /// <summary>
    /// Create a tournament. It verify first that the name is unique
    /// </summary>
    /// <param name="tournaments"></param>
    /// <returns>An empty string is everything is correct. An error message in other case</returns>
    public async Task<string> CreateTournaments(Tournament tournament)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        int result = 0;
        try
        {
            result = await db.ExecuteAsync(@"
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
        }
        catch
        {}
        if (result == 0)
        {
            return "Ya existe un torneo con ese nombre";
        }
        return "";
    }

    public async Task<string> DeleteTournament(Tournament tournament)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        int result = 0;
        result = await db.ExecuteScalarAsync<int>(@"
select count(*) 
from TournamentsUserTeams
where TournamentId = @Id
            ", new
            {
                tournament.Id
            });
        if (result >= 0)
        {
            return "No se puede borrar un torneo con usuarios";
        }    
        result = await db.ExecuteAsync(@"
delete from tournaments
where Id = @Id",
            new
            {
                tournament.Id
            });        
        return "";
    }


}