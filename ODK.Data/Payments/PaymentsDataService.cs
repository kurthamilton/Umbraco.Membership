using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ODK.Data.Payments
{
    public class PaymentsDataService
    {
        private readonly string _connectionString;

        public PaymentsDataService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IReadOnlyCollection<Payment>> GetMemberPayments(int memberId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SELECT Amount, Date FROM dbo.odkPayments WHERE MemberId = @MemberId", connection))
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
