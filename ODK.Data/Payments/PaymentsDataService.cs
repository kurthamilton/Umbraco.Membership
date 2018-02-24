using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ODK.Data.Payments
{
    public class PaymentsDataService : DataServiceBase
    {
        private const string InsertColumns = "memberId, memberName, currencyCode, amount, token";
        private const string ReadColumns = "id, " + InsertColumns + ", date";
        private const string TableName = "dbo.odkPayments";

        public PaymentsDataService(string connectionString)
            : base(connectionString)
        {
        }

        public void CompletePayment(int id, string currencyCode, double amount)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"UPDATE {TableName} SET Complete = 1, Status = 'Complete', currencyCode = @CurrencyCode, amount = @Amount WHERE id = @Id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("@CurrencyCode", SqlDbType.NVarChar).Value = currencyCode;
                    command.Parameters.Add("@Amount", SqlDbType.Float).Value = amount;

                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreatePayment(Payment payment)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"INSERT INTO {TableName} ({InsertColumns}) VALUES (@MemberId, @MemberName, @CurrencyCode, @Amount, @Token)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = payment.MemberId;
                    command.Parameters.Add("@MemberName", SqlDbType.NVarChar).Value = payment.MemberName;
                    command.Parameters.Add("@CurrencyCode", SqlDbType.NVarChar).Value = payment.CurrencyCode;
                    command.Parameters.Add("@Amount", SqlDbType.Float).Value = payment.Amount;
                    command.Parameters.Add("@Token", SqlDbType.UniqueIdentifier).Value = payment.Token;

                    command.ExecuteNonQuery();
                }
            }
        }

        public Payment GetIncompletePayment(Guid token)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $" SELECT {ReadColumns} FROM {TableName}  WHERE token = @Token AND Status IS NULL";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Token", SqlDbType.UniqueIdentifier).Value = token;

                    Payment payment = ReadRecord(command, ReadPayment);
                    return payment;
                }
            }
        }

        public IReadOnlyCollection<Payment> GetCompletePayments(int memberId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $" SELECT {ReadColumns} FROM {TableName}  WHERE memberId = @MemberId AND Complete = 1";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = memberId;

                    IReadOnlyCollection<Payment> payments = ReadRecords(command, ReadPayment);
                    return payments;
                }
            }
        }

        private static Payment ReadPayment(SqlDataReader reader)
        {
            return new Payment
            (
                reader.GetInt32(reader.GetOrdinal("id")),
                reader.GetInt32(reader.GetOrdinal("memberId")),
                reader.GetString(reader.GetOrdinal("memberName")),
                reader.GetString(reader.GetOrdinal("currencyCode")),
                reader.GetDouble(reader.GetOrdinal("amount")),
                reader.GetDateTime(reader.GetOrdinal("date"))
            );
        }
    }
}
