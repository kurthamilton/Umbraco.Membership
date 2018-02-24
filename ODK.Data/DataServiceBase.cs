using System;
using System.Collections.Generic;
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

        protected T ReadRecord<T>(SqlCommand command, Func<SqlDataReader, T> read)
        {
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return default(T);
            }

            return read(reader);
        }

        protected IReadOnlyCollection<T> ReadRecords<T>(SqlCommand command, Func<SqlDataReader, T> read)
        {
            List<T> records = new List<T>();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                T record = read(reader);
                records.Add(record);
            }

            return records;
        }
    }
}
