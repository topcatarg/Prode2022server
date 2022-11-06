namespace Prode2022Server.Services;

using Dapper;
using Microsoft.Data.Sqlite;
using Prode2022Server.Models;
using Prode2022Server.Models.UserData;
using Prode2022Server.Models.Tournaments;
using System.Collections.Immutable;

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

    public async Task<string> UpdateTournamentAsync(Tournament tournament)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        int result = 0;
        try
        {
            result = await db.ExecuteAsync(@"
update tournaments
set Name = @Name,
    Password = @Password
where
    Id = @Id
",
                new
                {
                    tournament.Name,
                    tournament.Password,
                    tournament.Id
                });
        }
        catch
        { }
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
        if (result > 0)
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

    /// <summary>
    /// Get the list of tournaments the user is enrolled on.
    /// </summary>
    /// <param name="UserId">User Id</param>
    /// <returns>The list of tournaments</returns>
    public async Task<List<UserTournament>> GetUserTournamentsAsync(int UserId)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var v = await db.QueryAsync<UserTournament>(@"
select T.Id, U.TeamName as TeamName, TU.Name as TournamentName, T.TournamentId, T.UserTeamId
from TournamentsUserTeams T 
    inner join UserTeams U on U.Id = T.UserTeamId 
    inner join Tournaments TU on T.TournamentId = TU.Id
where U.UserId = @Id",
        new
        {
            Id = UserId,
        });
        return v.ToList();
    }

    public async Task<List<UserTournament>> GetOtherTournamentsForUserAsync(int UserId)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var v = await db.QueryAsync<UserTournament>(@"
select id as TournamentId, Name as TournamentName, Password
from Tournaments 
where Id not in (select TournamentId from TournamentsUserTeams TU inner join UserTeams U on U.Id = TU.UserTeamId where U.UserId = @Id)",
        new
        {
            Id = UserId,
        });
        return v.ToList();
    }

    public async Task<(string error, int NewId)> CreateTeamAsync(UserTournament userTournament)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        int result = 0;
        try
        {
            result = await db.ExecuteAsync(@"
insert into UserTeams (TeamName, UserId)
values
    (@TeamName, @UserId)",
                new
                {
                    userTournament.TeamName,
                    userTournament.UserId
                });
            result = await db.ExecuteScalarAsync<int>(@"
select last_insert_rowid()");
        }
        catch
        {}
        if (result == 0)
        {
            return new(
                "El nombre de equipo ya existe.",
                0
            );
        }
        return new("", result);
    }

    public async Task<string> RegisterUserInTournamentAsync(UserTournament userTournament)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        int result = 0;
        try
        {
            result = await db.ExecuteAsync(@"
Insert into TournamentsUserTeams (TournamentId, UserTeamId)
values
    (@TournamentId, @UserTeamId)",
                new
                {
                    userTournament.TournamentId,
                    userTournament.UserTeamId
                });
            result = await db.ExecuteAsync(@"
insert into UserForecast (TeamId, MatchId)
select @UserTeamId, Id from Matches",
                new
                {
                    userTournament.UserTeamId
                });
        }
        catch
        {}
        if (result == 0)
        {
            return "Ocurrio un error al inscribirse en el torneo.";
        }
        return "";
    }

    public async Task<ImmutableArray<TournamentPositions>> GetTournamentPositionsAsync(int TournamentId)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var v = await db.QueryAsync<TournamentPositions>(@"
select 
    row_number() over (order by T.Score DESC) Position,
    U.TeamName TeamName, T.Score Score, U.UserId UserId
from TournamentsUserTeams T 
	inner join UserTeams U on T.UserTeamId = U.Id
WHERE T.TournamentId = @TournamentId
ORDER by T.Score Desc",
            new
            {
                TournamentId
            });
        return v.ToImmutableArray();
    }

}