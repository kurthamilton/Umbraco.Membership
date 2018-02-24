using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ODK.Data.Events
{
    public class EventsDataService : DataServiceBase
    {
        private const string TableName = "dbo.odkEventResponses";

        public EventsDataService(string connectionString)
            : base(connectionString)
        {
        }

        public IReadOnlyCollection<EventResponse> GetEventResponses(int eventId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                using (SqlCommand command = new SqlCommand($"SELECT memberId, eventId, responseTypeId FROM {TableName} WHERE eventId = @EventId", connection))
                {
                    command.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;

                    IReadOnlyCollection<EventResponse> eventResponses = ReadRecords(command, ReadEventResponse);
                    return eventResponses;
                }
            }
        }

        public IReadOnlyCollection<EventResponse> GetMemberResponses(int memberId, IEnumerable<int> eventIds)
        {
            string eventIdString = string.Join(",", eventIds);
            if (string.IsNullOrEmpty(eventIdString))
            {
                return new EventResponse[] { };
            }

            using (SqlConnection connection = OpenConnection())
            {
                string sql = $" SELECT memberId, eventId, responseTypeId" +
                             $" FROM {TableName}" +
                             $" WHERE memberId = @memberId AND eventId IN (" + eventIdString + ")";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = memberId;

                    IReadOnlyCollection<EventResponse> eventResponses = ReadRecords(command, ReadEventResponse);
                    return eventResponses;
                }
            }
        }

        public void UpdateEventResponse(EventResponse eventResponse)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $" IF NOT EXISTS(SELECT * FROM {TableName} WHERE eventId = @EventId AND memberId = @MemberId)" +
                             $" INSERT INTO {TableName} (eventId, memberId, responseTypeId) VALUES (@EventId, @MemberId, @ResponseTypeId)" +
                             $" ELSE" +
                             $" UPDATE {TableName} SET responseTypeId = @ResponseTypeId WHERE eventId = @EventId AND memberId = @MemberId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EventId", SqlDbType.Int).Value = eventResponse.EventId;
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = eventResponse.MemberId;
                    command.Parameters.Add("@ResponseTypeId", SqlDbType.Int).Value = eventResponse.ResponseTypeId;

                    command.ExecuteNonQuery();
                }
            }
        }

        private static EventResponse ReadEventResponse(SqlDataReader reader)
        {
            return new EventResponse
            {
                EventId = reader.GetInt32(reader.GetOrdinal("eventId")),
                MemberId = reader.GetInt32(reader.GetOrdinal("memberId")),
                ResponseTypeId = reader.GetInt32(reader.GetOrdinal("responseTypeId"))
            };
        }
    }
}
