using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ODK.Data
{
    public abstract class DataServiceBase
    {
        private readonly string _connectionString;

        protected DataServiceBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected async Task<SqlConnection> OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
