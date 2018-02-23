using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ODK.Data.Payments
{
    public class PaymentsDataService : DataServiceBase
    {
        private const string PaymentRequestsTableName = "dbo.odkPaymentRequests";
        private const string PaymentsTableName = "dbo.odkPayments";

        public PaymentsDataService(string connectionString)
            : base(connectionString)
        {
        }

        public void CreatePaymentRequest(PaymentRequest paymentRequest)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"INSERT INTO {PaymentRequestsTableName} (memberId, name, token, amount, secret) " +
                             $"VALUES (@MemberId, @MemberName, @Token, @Amount, @Secret)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = paymentRequest.MemberId;
                    command.Parameters.Add("@MemberName", SqlDbType.NVarChar).Value = paymentRequest.MemberName;
                    command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = paymentRequest.Token.ToString();
                    command.Parameters.Add("@Amount", SqlDbType.Float).Value = paymentRequest.Amount;
                    command.Parameters.Add("@Secret", SqlDbType.NVarChar).Value = paymentRequest.Secret;

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePaymentRequest(PaymentRequest paymentRequest)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"DELETE {PaymentRequestsTableName} " +
                             $"WHERE memberId = @MemberId AND Token = @Token";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = paymentRequest.MemberId;
                    command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = paymentRequest.Token.ToString();

                    command.ExecuteNonQuery();
                }
            }
        }

        public PaymentRequest GetPaymentRequest(Guid token, string secret)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $" SELECT memberId, name, token, amount, secret " +
                             $" FROM {PaymentRequestsTableName} " +
                             $" WHERE token = @Token AND secret = @Secret";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = token.ToString();
                    command.Parameters.Add("@Secret", SqlDbType.NVarChar).Value = secret;

                    SqlDataReader reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new PaymentRequest
                    {
                        Amount = reader.GetDouble(reader.GetOrdinal("amount")),
                        MemberId = reader.GetInt32(reader.GetOrdinal("memberId")),
                        MemberName = reader.GetString(reader.GetOrdinal("name")),
                        Secret = reader.GetString(reader.GetOrdinal("secret")),
                        Token = reader.GetGuid(reader.GetOrdinal("token"))
                    };
                }
            }
        }

        public IReadOnlyCollection<Payment> GetPayments(int memberId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                using (SqlCommand command = new SqlCommand($"SELECT Amount, Date FROM {PaymentsTableName} WHERE MemberId = @MemberId", connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = memberId;

                    SqlDataReader reader = command.ExecuteReader();

                    List<Payment> payments = new List<Payment>();

                    while (reader.Read())
                    {
                        Payment payment = ReadPayment(reader);
                        payments.Add(payment);
                    }

                    return payments;
                }
            }
        }

        public void LogPayment(Payment payment)
        {
            using (SqlConnection connection = OpenConnection())
            {
                using (SqlCommand command = new SqlCommand($"INSERT INTO {PaymentsTableName} (memberId, name, amount, date) VALUES (@MemberId, @MemberName, @Amount, @Date)", connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = payment.MemberId;
                    command.Parameters.Add("@MemberName", SqlDbType.NVarChar).Value = payment.MemberName;
                    command.Parameters.Add("@Amount", SqlDbType.Float).Value = payment.Amount;
                    command.Parameters.Add("@Date", SqlDbType.DateTime).Value = payment.Date;

                    command.ExecuteNonQuery();
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
