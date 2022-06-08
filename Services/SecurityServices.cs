namespace Prode2022Server.Services
{
    public class SecurityServices
    {
        private readonly DbService database;

        public SecurityServices(DbService dbService)
        {
            database = dbService;
        }
    }
}
