using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ODK.Data.Events
{
    public class EventsDataService : DataServiceBase
    {
        private const string EventResponsesTableName = "dbo.odkEventResponses";

        public EventsDataService(string connectionString)
            : base(connectionString)
        {
        }

        public async Task<IReadOnlyCollection<EventResponse>> GetEventResponses(int eventId)
        {
            using (SqlConnection connection = await OpenConnection())
            {
                using (SqlCommand command = new SqlCommand($"SELECT memberId, responseTypeId FROM {EventResponsesTableName} WHERE eventId = @EventId", connection))
                {
                    command.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    List<EventResponse> eventResponses = new List<EventResponse>();

                    while (await reader.ReadAsync())
                    {
                        EventResponse payment = ReadEventResponse(reader);
                        eventResponses.Add(payment);
                    }

                    return eventResponses;
                }
            }
        }

        public async Task UpdateEventResponse(EventResponse eventResponse)
        {
            using (SqlConnection connection = await OpenConnection())
            {
                string sql = $" IF NOT EXISTS(SELECT * FROM {EventResponsesTableName} WHERE eventId = @EventId AND memberId = @MemberId)" +
                             $" INSERT INTO {EventResponsesTableName} (eventId, memberId, responseTypeId) VALUES (@EventId, @MemberId, @ResponseTypeId)" +
                             $" ELSE" +
                             $" UPDATE {EventResponsesTableName} SET responseTypeId = @ResponseTypeId WHERE eventId = @EventId AND memberId = @MemberId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EventId", SqlDbType.Int).Value = eventResponse.EventId;
                    command.Parameters.Add("@MemberId", SqlDbType.Int).Value = eventResponse.MemberId;
                    command.Parameters.Add("@ResponseTypeId", SqlDbType.Int).Value = eventResponse.ResponseTypeId;

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private static EventResponse ReadEventResponse(SqlDataReader reader)
        {
            return new EventResponse
            {
                MemberId = reader.GetInt32(reader.GetOrdinal("memberId")),
                ResponseTypeId = reader.GetInt32(reader.GetOrdinal("responseTypeId"))
            };
        }
    }
}
