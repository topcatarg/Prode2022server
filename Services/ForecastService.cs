namespace Prode2022Server.Services;

using Dapper;
using System.Collections.Immutable;
using Prode2022Server.Models;

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
    public async Task<ImmutableArray<UserForecastGroups>> GetFixtureGroupsAsync()
    {
        using var db = _dbService.SimpleDbConnection();
        var v = await db.QueryAsync<UserForecastGroups>(@"
select 
    Id as ID,
    Name as GroupName
from
    FixtureGroupsName");
        return v.ToImmutableArray();
    }
}

