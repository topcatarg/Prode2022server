namespace Prode2022Server.Services;

using Prode2022Server.Models.AdminSite;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Immutable;
public class AdminSiteServices
{

    private readonly DbService database;

    public event Func<Task>? NotifyProgress;
    public int CalculateState = 0;
    public int MaxState = 0;
    public string CalculateMatchsNotes = "";
    public AdminSiteServices(DbService dbService)
    {
        database = dbService;
    }

    public async Task<bool> CleanRefreshTokensAsync()
    {
        await Task.Delay(4000);
        return true;
    }

    public async Task<SiteData> GetAdminSiteData(SiteDataEnum value)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var v = await db.QueryFirstAsync<SiteData>(@"
select *
from AdminSiteData
where
    Id = @value", new
        {
            value
        });
        return v;
    }

    public async Task<Boolean> StoreTimeDifferenceAsync(int Difference)
    {
        using SqliteConnection db = database.SimpleDbConnection();
        var v = await db.ExecuteAsync(@"
update AdminSiteData 
set data = @Difference
where Id = @Id",
            new
            {
                Difference,
                Id = SiteDataEnum.TimeDifference
            });
        return v > 0;
    }

    public async Task<string> CalculateMatchs()
    {
        CalculateMatchsNotes = "Calculando partidos";
        CalculateState = 0;
        MaxState = 1;
        await CallProgress();
        using SqliteConnection db = database.SimpleDbConnection();
        //Calculate matchs
        var results = (await db.QueryAsync<MatchsScore>(@"
select  m.id, TeamId,ScorePerGame,
case 
	when m.team1goals = uf.team1goals and m.team2goals = uf.team2goals then 3
	when m.team1goals = m.team2goals and uf.team1goals = uf.team2goals then 1
	when m.team1goals - m.team2goals = uf.team1goals - uf.team2goals then 2
	when (m.team1goals > m.team2goals and uf.team1goals > uf.team2goals)
		or (m.team1goals < m.team2goals and uf.team1goals < uf.team2goals) then 1
	else 0 
end as score
from 
(select id, team1goals, team2goals
from Matches
where closed = 1) m inner join UserForecast uf on m.id = uf.MatchId")).ToImmutableList();
        //Calculate users forecast
        CalculateState = 0;
        CalculateMatchsNotes = "Obteniendo lista a actualizar";
        MaxState = 1;
        await CallProgress();
        var ResultsToUpdate = results.Where(x => x.Score != x.ScorePerGame).ToImmutableList();
        CalculateState = 0;
        CalculateMatchsNotes = "Actualizando partidos de los usuarios";
        MaxState = ResultsToUpdate.Count();
        await CallProgress();
        foreach (var r in ResultsToUpdate)
        {
            var rest = await db.ExecuteAsync(@"
Update UserForecast 
Set ScorePerGame = @score
Where TeamId = @TeamId and MatchId = @matchid", new
            {
                r.Score,
                r.TeamId,
                matchid = r.Id
            });
            if (rest == 0)
            {
                return "Hubo un error calculando";
            }

        }
        return "";
        //Calculate totals for every user
    }

    private async Task CallProgress()
    {
        if (NotifyProgress != null)
        {
            await NotifyProgress.Invoke();
        }
    }
}
