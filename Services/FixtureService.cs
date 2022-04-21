using System.Collections.Immutable;
using Prode2022Server.Models;
using Dapper;

namespace Prode2022Server.Services
{
    public class FixtureService
    {
        private readonly DbService _dbService;

        public FixtureService(DbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ImmutableArray<Matchs>> GetAllMatchs()
        {
            
            using var db = _dbService.SimpleDbConnection();
            var v = await db.QueryAsync<Matchs>(@"
select *,
strftime(""%d/%m %H:%M"",date) as StandardDate,
(select Team from Teams t where t.id = m.team1) as Team1Name,
(select Team from Teams t where t.id = m.team2) as Team2Name,
(select Code from Teams t where t.id = m.team1) as Team1Flag,
(select Code from Teams t where t.id = m.team2) as Team2Flag
from Matches m
order by date");
                return v.ToImmutableArray();
        }
    }

}