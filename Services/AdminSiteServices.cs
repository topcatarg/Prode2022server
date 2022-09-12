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
}
