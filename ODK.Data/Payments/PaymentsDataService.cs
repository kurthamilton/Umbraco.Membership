using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ODK.Data.Payments
{
    public class PaymentsDataService : DataServiceBase
    {
        private const string InsertColumns = "id, memberId, memberName, currencyCode, identifier";
        private const string ReadColumns = InsertColumns + ", date";
        private const string TableName = "dbo.odkPayments";

        public PaymentsDataService(string connectionString)
            : base(connectionString)
        {
        }

        public void CompletePayment(Guid id, string currencyCode, double amount)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"UPDATE {TableName} SET Complete = 1, Status = 'Complete', currencyCode = @CurrencyCode WHERE id = @Id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                    command.Parameters.Add("@CurrencyCode", SqlDbType.NVarChar).Value = currencyCode;

                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreatePayment(Payment payment)
        {
            using (SqlConnection connection = OpenConnection())
            {
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    string sql = $"INSERT INTO {TableName} ({InsertColumns}) VALUES (@Id, @MemberId, @MemberName, @CurrencyCode, @Identifier)";
                    using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                    {
                        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = payment.Id;
                        command.Parameters.Add("@MemberId", SqlDbType.Int).Value = payment.MemberId > 0 ? (object)payment.MemberId : DBNull.Value;
                        command.Parameters.Add("@MemberName", SqlDbType.NVarChar).Value = !string.IsNullOrEmpty(payment.MemberName) ? (object)payment.MemberName : DBNull.Value;
                        command.Parameters.Add("@CurrencyCode", SqlDbType.NVarChar).Value = payment.CurrencyCode;
                        command.Parameters.Add("@Identifier", SqlDbType.NVarChar).Value = payment.Identifier;

                        command.ExecuteNonQuery();
                    }

                    foreach (PaymentDetail paymentDetail in payment.Details)
                    {
                        sql = "INSERT INTO dbo.odkPaymentDetails (paymentId, amount, nodeId) VALUES (@PaymentId, @Amount, @NodeId)";
                        using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.Add("@PaymentId", SqlDbType.UniqueIdentifier).Value = paymentDetail.PaymentId;
                            command.Parameters.Add("@Amount", SqlDbType.Float).Value = paymentDetail.Amount;
                            command.Parameters.Add("@NodeId", SqlDbType.Int).Value = paymentDetail.NodeId;

                            command.ExecuteNonQuery();
                        }
                    }

                    TryCommitTransaction(transaction);
                }
            }
        }

        public Payment GetIncompletePayment(string identifier)
        {
            using (SqlConnection connection = OpenConnection())
            {
                IReadOnlyCollection<PaymentDetail> paymentDetails = GetPaymentDetails(identifier, connection);

                string sql = $" SELECT {ReadColumns} FROM {TableName}  WHERE identifier = @Identifier AND Status IS NULL";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Identifier", SqlDbType.NVarChar).Value = identifier;

                    Payment payment = ReadRecord(command, reader => ReadPayment(reader, paymentDetails));
                    return payment;
                }
            }
        }

        public IReadOnlyCollection<Payment> GetCompletePayments(int memberId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                IReadOnlyCollection<PaymentDetail> paymentDetails = GetPaymentDetails(memberId, connection);

                string sql = $" SELECT {ReadColumns} FROM {TableName} WHERE memberId = @MemberId AND Complete = 1";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = memberId;

                    IReadOnlyCollection<Payment> payments = ReadRecords(command, reader => ReadPayment(reader, paymentDetails));
                    return payments;
                }
            }
        }

        private static Payment ReadPayment(SqlDataReader reader, IReadOnlyCollection<PaymentDetail> paymentDetails)
        {
            Guid paymentId = reader.GetGuid(reader.GetOrdinal("id"));

            return new Payment
            (
                paymentId,
                reader.GetString(reader.GetOrdinal("identifier")),
                reader.GetValue<int?>("memberId", (r, i) => r.GetInt32(i)),
                reader.GetValue("memberName", (r, i) => r.GetString(i)),
                reader.GetString(reader.GetOrdinal("currencyCode")),
                reader.GetDateTime(reader.GetOrdinal("date")),
                paymentDetails.Where(x => x.PaymentId == paymentId)
            );
        }

        private static PaymentDetail ReadPaymentDetail(SqlDataReader reader)
        {
            return new PaymentDetail
            (
                reader.GetDouble(reader.GetOrdinal("amount")),
                reader.GetInt32(reader.GetOrdinal("nodeId")),
                reader.GetGuid(reader.GetOrdinal("paymentId"))
            );
        }

        private IReadOnlyCollection<PaymentDetail> GetPaymentDetails(int memberId, SqlConnection connection)
        {
            string sql = $" SELECT amount, paymentId, nodeId " +
                         $" FROM {TableName} " +
                         $" JOIN dbo.odkPaymentDetails ON {TableName}.id = dbo.odkPaymentDetails.paymentId " +
                         $" WHERE memberId = @MemberId";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@MemberId", SqlDbType.Int).Value = memberId;

                IReadOnlyCollection<PaymentDetail> records = ReadRecords(command, ReadPaymentDetail);
                return records;
            }
        }

        private IReadOnlyCollection<PaymentDetail> GetPaymentDetails(string identifier, SqlConnection connection)
        {
            string sql = $" SELECT amount, paymentId, nodeId " +
                         $" FROM {TableName} " +
                         $" JOIN dbo.odkPaymentDetails ON {TableName}.id = dbo.odkPaymentDetails.paymentId " +
                         $" WHERE identifier = @Identifier";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Identifier", SqlDbType.NVarChar).Value = identifier;

                IReadOnlyCollection<PaymentDetail> records = ReadRecords(command, ReadPaymentDetail);
                return records;
            }
        }
    }
}
