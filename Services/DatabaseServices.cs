using Microsoft.Data.Sqlite;

namespace Prode2022Server.Services
{

    public class DbService
    {

        public static string DbFile
        {
            get { return Environment.CurrentDirectory + "\\Database\\prode2022.sqlite"; }
        }

        public SqliteConnection SimpleDbConnection()
        {
            //var v = new SQLiteConnection();
            return new SqliteConnection("Data Source=" + DbFile);
        }

        public DbService(IConfiguration configuration)
        {
            if (!File.Exists(DbFile))
            {
                using (var cnn = SimpleDbConnection())
                {
                    cnn.Open();
                }
            }
        }

    }
}