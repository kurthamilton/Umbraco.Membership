using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ODK.Data.Payments
{
    public class PaymentsDataService
    {
        private const string TableName = "dbo.odkPayments";

        private readonly string _connectionString;

        public PaymentsDataService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IReadOnlyCollection<Payment>> GetPayments(int memberId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand($"SELECT Amount, Date FROM {TableName} WHERE MemberId = @MemberId", connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = memberId;

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    List<Payment> payments = new List<Payment>();

                    while (await reader.ReadAsync())
                    {
                        Payment payment = ReadPayment(reader);
                        payments.Add(payment);
                    }

                    return payments;
                }
            }
        }

        public async Task LogPayment(Payment payment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand($"INSERT INTO {TableName} (memberId, name, amount, date) VALUES (@MemberId, @MemberName, @Amount, @Date)", connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = payment.MemberId;
                    command.Parameters.Add("@MemberName", SqlDbType.NVarChar).Value = payment.MemberName;
                    command.Parameters.Add("@Amount", SqlDbType.Float).Value = payment.Amount;
                    command.Parameters.Add("@Date", SqlDbType.DateTime).Value = payment.Date;

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private static Payment ReadPayment(SqlDataReader reader)
        {
            return new Payment
            {
                Amount = reader.GetDouble(reader.GetOrdinal("Amount")),
                Date = reader.GetDateTime(reader.GetOrdinal("Date"))
            };
        }
    }
}
