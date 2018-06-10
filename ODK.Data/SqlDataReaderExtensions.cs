using System;
using System.Data.SqlClient;

namespace ODK.Data
{
    public static class SqlDataReaderExtensions
    {
        public static T GetValue<T>(this SqlDataReader reader, string name, Func<SqlDataReader, int, T> getValue)
        {
            int ordinal = reader.GetOrdinal(name);
            if (reader[ordinal] == DBNull.Value)
            {
                return default(T);
            }

            return getValue(reader, ordinal);
        }
    }
}
