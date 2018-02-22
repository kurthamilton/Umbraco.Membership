using System.Data.SqlClient;

namespace ODK.Data
{
    public abstract class DataServiceBase
    {
        private readonly string _connectionString;

        protected DataServiceBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
