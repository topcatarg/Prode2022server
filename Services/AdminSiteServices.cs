namespace Prode2022Server.Services;

using System.Threading.Tasks;
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
}
