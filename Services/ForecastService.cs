namespace Prode2022Server.Services;

using Dapper;
using System.Collections.Immutable;
using Prode2022Server.Models.UserForecast;

public class ForecastService
{

    private readonly DbService _dbService;

    public ForecastService(DbService dbService)
    {
        _dbService = dbService;
    }

    /// <summary>
    /// Get a list of groups, order by next matches. Also, add the finished groups
    /// </summary>
    /// <returns></returns>
    public async Task<ImmutableArray<FixtureGroup>> GetFixtureGroupsAsync()
    {
        using var db = _dbService.SimpleDbConnection();
        var v = await db.QueryAsync<FixtureGroup>(@"
select distinct stage as stageid, 
	(select f.name from FixtureGroupsName f where f.id = m.stage) as stagename,
    closed
from Matches m 
order by
	m.Closed, m.date");
        return v.ToImmutableArray();
    }

    public async Task<ImmutableArray<Match>> GetNextMatchesAsync(FiltersNotifier f, int userId)
    {
        string whereclausules = "";
        if (f.StageId != 0)
        {
            whereclausules += " and M.stage = @StageId ";
        }
        if (f.Closed)
        {
            whereclausules += " and M.Closed = @Closed";
        }
        if (f.UserTeamId != 0)
        {
            whereclausules += " and UT.Id = @UserTeamId";
        }
        using var db = _dbService.SimpleDbConnection();
        var v = await db.QueryAsync<Match>(@$"
select U.Id,
	U.Team1Goals,
	U.Team2Goals,
	T1.Team Team1Name,
	T1.Code Team1Flag,
	T2.Team Team2Name,
	T2.Code Team2Flag,
	U.Team1Goals,
	U.Team2Goals,
	M.Closed,
	UT.TeamName MyTeamName,
	UT.Id MyTeamId,
    F.Name StageName,
    M.Date Date
from 
	UserForecast U inner join Matches M on M.Id = U.MatchId
	inner join  Teams T1 on T1.ID = M.Team1
	inner join Teams T2 on T2.ID = M.Team2
	inner join UserTeams UT on UT.Id = U.TeamId
    inner join FixtureGroupsName F on F.Id = M.Stage
where U.TeamId in (SELECT Id from UserTeams where UserId= @userId)
    {whereclausules}
order by M.Closed, M.Date",
            new 
            {
                userId,
                f.Closed,
                f.StageId,
                f.UserTeamId
            });
        return v.ToImmutableArray();
    }

    public async Task<ImmutableArray<UserTeam>> GetUserTeamsAsync(int userId)
    {
        using var db = _dbService.SimpleDbConnection();
        var v = await db.QueryAsync<UserTeam>(@"
select id, TeamName
from UserTeams
where UserId = @userId",
            new
            {
                userId
            });
        return v.ToImmutableArray();
    }
}

