namespace Prode2022Server.Services;

using Prode2022Server.Models.AdminSite;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
public class AdminSiteServices
{
    
    private readonly DbService database;

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
        return (await db.QueryFirstAsync<SiteData>(@"
select *
from AdminSiteData
where
    Id = @value", new
        {
            value
        }));
    }
}
