using System.Collections.Immutable;
using Prode2022Server.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace Prode2022Server.Services
{
    public class DataAdminServices
    {
        private readonly DbService database;

        public DataAdminServices(DbService dbService)
        {
            database = dbService;
        }

        /// <summary>
        /// Return a list of Teams for the tournament
        /// </summary>
        /// <returns>A list of teams that are in the championship tournament</returns>
        public async Task<List<Country>> GetAllTournamentTeamsAsync()
        {
            using SqliteConnection db = database.SimpleDbConnection();
            return (await db.QueryAsync<Country>(
                @"Select * from teams order by ID"
            )).ToList();
        }

        public async Task<bool> UpSertTournamentTeamAsync(Country country)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            int result; 
            if (country.ID == 0)
            {
                result = await db.ExecuteAsync(
                @"insert into Teams (Team,code) values (@Team, @Code)",
                new {
                    country.Team,
                    country.Code
                });
            }
            else
            {
                result = await db.ExecuteAsync(
                @"update teams set team = @Team, code = @Code where id = @ID",
                new {
                    country.ID,
                    country.Team,
                    country.Code
                });
            }
            return result > 0;
        }
    
        public async Task<bool> DeleteCountryAsync(Country country)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            //check if can delete on other tables
            int result;
            result = await db.ExecuteScalarAsync<int>(
                @"Select count(Team1)+count(Team2) from Matches where Team1 = @team or Team2 = @team",
                new {
                    team = country.ID
                }
            );
            if (result > 0)
            {
                return false;
            }
            result = await db.ExecuteAsync(
                @"delete from Teams where Id = @Id",
                new {
                    Id = country.ID
                }
            );
            return result>0;
        }
    
    
        public async Task<List<FixtureGroups>> GetAllFixtureGroupsAsync()
        {
            using SqliteConnection db = database.SimpleDbConnection();
            var data = await db.QueryAsync<FixtureGroups>(
                @"Select id, Name as GroupName from FixtureGroupsName order by ID"
            );
            db.Close();
            return data.ToList();
        }

        /// <summary>
        /// Delete a fixture group
        /// </summary>
        /// <param name="fixtureGroups">The fixture group class to delete</param>
        /// <returns>True is deleted ocurred</returns>
        public async Task<bool> DeleteFixtureGroupAsync(FixtureGroups fixtureGroups)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            //check if can delete on other tables
            int result;
            result = await db.ExecuteScalarAsync<int>(
                @"Select count(stage) from Matches where stage = @id",
                new {
                    id = fixtureGroups.ID
                }
            );
            if (result > 0)
            {
                return false;
            }
            result = await db.ExecuteAsync(
                @"delete from FixtureGroupsName where Id = @Id",
                new {
                    Id = fixtureGroups.ID
                }
            );
            return result>0;
        }
    
        public async Task<bool> UpSertFixtureGroupsAsync(FixtureGroups fixtureGroups)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            int result; 
            if (fixtureGroups.ID == 0)
            {
                result = await db.ExecuteAsync(
                @"insert into FixtureGroupsName (Name) values (@name)",
                new {
                    name = fixtureGroups.GroupName
                });
            }
            else
            {
                result = await db.ExecuteAsync(
                @"update FixtureGroupsName set Name = @name where id = @id",
                new {
                    id = fixtureGroups.ID,
                    name = fixtureGroups.GroupName
                });
            }
            return result > 0;
        }
    
        public async Task<List<FixtureMatch>> GetAllMatchsAsync()
        {
            using SqliteConnection db = database.SimpleDbConnection();
            var data = await db.QueryAsync<FixtureMatch>(@"
                select
Id,
strftime(""%d/%m"",date) as date,
strftime(""%H:%M"",date) as time,
Team1,
Team2,
Stage,
(select name from fixturegroupsname f where f.id = m.stage) as Stagename,
(select Team from Teams t where t.id = m.team1) as Team1Name,
(select Team from Teams t where t.id = m.team2) as Team2Name,
(select Code from Teams t where t.id = m.team1) as Team1Flag,
(select Code from Teams t where t.id = m.team2) as Team2Flag
from Matches m
order by date"
            );
            db.Close();
            return data.ToList();
        }

        public async Task<List<FixtureMatch>> GetAllFixtureMatchs()
        {
            using SqliteConnection db = database.SimpleDbConnection();
            var data = await db.QueryAsync<FixtureMatch>(@"
select 
    Id, 
    Team1, 
    Team2, 
    Stage, 
    Substr(Date,1,10) as Date, 
    Substr(Date,12,5) as Time 
from Matches"
            );
            db.Close();
            return data.ToList();
        }
    
        public async Task<bool> DeleteFixtureMatchAsync(FixtureMatch fixtureMatch)
        {
             using SqliteConnection db = database.SimpleDbConnection();
            //check if can delete on other tables
            int result;
            result = await db.ExecuteScalarAsync<int>(
                @"Select count(1) from UserForecast where MatchId = @id",
                new {
                    id = fixtureMatch.Id
                }
            );
            if (result > 0)
            {
                return false;
            }
            result = await db.ExecuteAsync(
                @"delete from Matches where Id = @Id",
                new {
                    Id = fixtureMatch.Id
                }
            );
            return result>0;
        }
    
        public async Task<bool> UpSertFixtureMatchAsync(FixtureMatch fixtureMatch)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            int result; 
            if (fixtureMatch.Id == 0)
            {
                result = await db.ExecuteAsync(
                @"
insert into Matches (Date,Team1,Team2,Stage,Team1Goals,Team2Goals) 
values (@date,@team1,@team2,@stage,0,0)",
                new {
                    date = $"2022-{fixtureMatch.Date!.Substring(3,2)}-{fixtureMatch.Date!.Substring(0,2)} {fixtureMatch.Time}",
                    team1 = fixtureMatch.Team1,
                    team2 = fixtureMatch.Team2,
                    stage = fixtureMatch.Stage
                });
            }
            else
            {
                result = await db.ExecuteAsync(@"
update matches set 
    Date = @date,
    Team1 = @team1,
    Team2 = @team2,
    Stage = @stage
where id = @id",
                new {
                    date = $"2022-{fixtureMatch.Date!.Substring(3,2)}-{fixtureMatch.Date!.Substring(0,2)} {fixtureMatch.Time}",
                    team1 = fixtureMatch.Team1,
                    team2 = fixtureMatch.Team2,
                    stage = fixtureMatch.Stage,
                    id = fixtureMatch.Id
                });
            }
            return result > 0;
        }
    
        public async Task<List<MatchResult>> GetMatchResultsAsync()
        {
            using SqliteConnection db = database.SimpleDbConnection();
            var data = await db.QueryAsync<MatchResult>(@"
                select
Id,
date,
coalesce(Team1Goals,0) as Team1Goals,
coalesce(Team2Goals,0) as Team2Goals,
(select name from fixturegroupsname f where f.id = m.stage) as Stagename,
(select Team from Teams t where t.id = m.team1) as Team1Name,
(select Team from Teams t where t.id = m.team2) as Team2Name
from Matches m
order by date"
            );
            db.Close();
            return data.ToList();
        }
    
        public async Task<bool> StoreMatchResultAsync(MatchResult matchResult)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            var result = await db.ExecuteAsync(@"
update matches set 
    Team1Goals = @Team1Goals,
    Team2Goals = @Team2Goals
where id = @id",
                new {
                    id = matchResult.Id,
                    Team1Goals = matchResult.Team1Goals,
                    Team2Goals = matchResult.Team2Goals,
                });
            return result > 0;
        }
    }
}