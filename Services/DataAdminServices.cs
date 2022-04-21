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

        public async Task<bool> AddCountry(Country country)
        {
            using SqliteConnection db = database.SimpleDbConnection();
            var count = await db.ExecuteAsync(
                @"insert into Teams (Team,code) values (@team, @code)",
                new {
                    team = country.Team,
                    code = country.Code
                }
            );
            return count > 0;
        }
    }
}