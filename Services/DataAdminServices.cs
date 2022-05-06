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

        public async Task<List<Country>> GetAllCountries()
        {
            using SqliteConnection db = database.SimpleDbConnection();
            var data = await db.QueryAsync<Country>(
                @"Select * from teams order by ID"
            );
            db.Close();
            return data.ToList();
        }

        public async Task<bool> UpSert(Country country)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            int result; 
            if (country.ID == 0)
            {
                result = await db.ExecuteAsync(
                @"insert into Teams (Team,code) values (@team, @code)",
                new {
                    team = country.Team,
                    code = country.Code
                });
            }
            else
            {
                result = await db.ExecuteAsync(
                @"update teams set team = @team, code = @code where id = @id",
                new {
                    id = country.ID,
                    team = country.Team,
                    code = country.Code
                });
            }
            return result > 0;
        }
    
        public async Task<bool> Delete(Country country)
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
    
    
        public async Task<List<FixtureGroups>> GetAllFixtureGroups()
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
        public async Task<bool> Delete(FixtureGroups fixtureGroups)
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
    
        public async Task<bool> UpSert(FixtureGroups fixtureGroups)
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
    }
}