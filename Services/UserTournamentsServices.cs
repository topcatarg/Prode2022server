namespace Prode2022Server.Services;

using Dapper;
using Microsoft.Data.Sqlite;
using Prode2022Server.Models;
using Prode2022Server.Models.UserData;
using Prode2022Server.Models.Tournaments;
using System.Collections.Immutable;
public class UserTournamentsServices
{

    private readonly DbService database;

    public UserTournamentsServices(DbService dbService)
    {
        database = dbService;
    }

    public async Task<string> DeleterUserForTournamentAsync(int TournamentId, int TeamId)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var result = await db.ExecuteScalarAsync<bool>(@"
select Started
from Tournaments
where Id = @TournamentId",
            new
            {
                TournamentId
            });
        if (result)
        {
            return "El torneo ya ha comenzado";
        }
        //delete from the tournament user table
        var count = await db.ExecuteAsync(@"
DELETE from TournamentsUserTeams
WHERE TournamentId = @TournamentId
	AND	UserTeamId = @TeamId",
            new
            {
                TournamentId,
                TeamId
            });
        if (count == 0)
        {
            return "No se pudo eliminar del torneo";
        }
        //check if the team is in another tournament. If not, delete all info of it.
        count = await db.ExecuteScalarAsync<int>(@"
SELECT count()
FROM TournamentsUserTeams
WHERE 	UserTeamId = @TeamId",
            new
            {
                TeamId
            });
        if (count == 0)
        {
            //delete the team data from all the tables. Also delete the forecast
            await db.ExecuteAsync(@"
DELETE from UserTeams
WHERE Id = @TeamId;

DELETE from UserForecast
WHERE TeamId=@TeamId",
                new
                {
                    TeamId
                });
        }
        return "";
    }

    public async Task<string> ChangeTeamNameAsync(int TeamId, string NewName)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        try
        {
            var resul = await db.ExecuteAsync(@"
UPDATE UserTeams
SET TeamName = @NewName
WHERE Id = @TeamId ",
                new
                {
                    TeamId,
                    NewName
                });
        }
        catch
        {
            return "El nombre ya existe";
        }
        return "";
    }

    public async Task<ImmutableArray<UserTournament>> GetUserTeamsAsync(int UserId)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var data = await db.QueryAsync<UserTournament>(@"
select Id UserTeamId, TeamName
from UserTeams
where UserId = @UserId",
            new
            {
                UserId
            });
        return data.ToImmutableArray();
    }

    public async Task<bool> AsignExistingTeamToTournamentAsync(int UserTeamId, int TournamentId)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var result = await db.ExecuteAsync(@"
INSERT INTO TournamentsUserTeams (TournamentId, UserTeamId)
VALUES (@TournamentId,@UserTeamId)",
            new
            {
                UserTeamId,
                TournamentId
            });
        return result > 0;
    }
}