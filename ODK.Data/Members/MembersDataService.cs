﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ODK.Data.Members
{
    public class MembersDataService : DataServiceBase
    {
        private const string MemberGroupsTableName = "dbo.odkMemberGroups";
        private const string MemberGroupMembersTableName = "dbo.odkMemberGroupMembers";
        private const string PasswordResetRequestsTableName = "dbo.odkPasswordResetRequests";

        public MembersDataService(string connectionString)
            : base(connectionString)
        {
        }

        public void AddMemberGroup(int chapterId, string name)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"INSERT INTO {MemberGroupsTableName} (chapterId, name) VALUES (@chapterId, @name)";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@chapterId", SqlDbType.Int).Value = chapterId;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddMemberToGroup(int memberId, int groupId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $" INSERT INTO {MemberGroupMembersTableName} (memberId, groupId) " +
                             $" SELECT @memberId, @groupId " +
                             $" WHERE NOT EXISTS(SELECT * FROM {MemberGroupMembersTableName} WHERE memberId = @memberId AND groupId = @groupId)";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;
                    cmd.Parameters.Add("@groupId", SqlDbType.Int).Value = groupId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddPasswordResetRequest(int memberId, DateTime created, DateTime expires, string token)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $" INSERT INTO {PasswordResetRequestsTableName} (memberId, created, expires, token) " +
                             $" VALUES (@memberId, @created, @expires, @token)";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;
                    cmd.Parameters.Add("@created", SqlDbType.DateTime).Value = created;
                    cmd.Parameters.Add("@expires", SqlDbType.DateTime).Value = expires;
                    cmd.Parameters.Add("@token", SqlDbType.NVarChar).Value = token;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteMemberGroup(int groupId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"DELETE {MemberGroupsTableName} WHERE groupId = @groupId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@groupId", SqlDbType.Int).Value = groupId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeletePasswordResetRequest(int passwordResetRequestId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"DELETE {PasswordResetRequestsTableName} WHERE passwordResetRequestId = @passwordResetRequestId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@passwordResetRequestId", SqlDbType.Int).Value = passwordResetRequestId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Dictionary<int, IReadOnlyCollection<MemberGroup>> GetMemberGroupMembers(int chapterId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $" SELECT Groups.groupId, Groups.name, Members.memberId" +
                             $" FROM {MemberGroupsTableName} Groups" +
                             $" JOIN {MemberGroupMembersTableName} Members ON Groups.groupId = Members.groupId" +
                             $" WHERE Groups.chapterId = @chapterId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@chapterId", SqlDbType.Int).Value = chapterId;

                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, List<MemberGroup>> memberGroupMembers = new Dictionary<int, List<MemberGroup>>();

                    while (reader.Read())
                    {
                        int memberId = reader.GetInt32(reader.GetOrdinal("memberId"));

                        if (!memberGroupMembers.ContainsKey(memberId))
                        {
                            memberGroupMembers.Add(memberId, new List<MemberGroup>());
                        }

                        MemberGroup group = ReadMemberGroup(reader);
                        memberGroupMembers[memberId].Add(group);
                    }

                    return memberGroupMembers.ToDictionary(x => x.Key, x => (IReadOnlyCollection<MemberGroup>)x.Value);
                }
            }
        }

        public IReadOnlyCollection<MemberGroup> GetMemberGroups(int chapterId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"SELECT groupId, name FROM {MemberGroupsTableName} WHERE chapterId = @chapterId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@chapterId", SqlDbType.Int).Value = chapterId;

                    return ReadRecords(cmd, ReadMemberGroup);
                }
            }
        }

        public IReadOnlyCollection<MemberGroup> GetMemberGroupsForMember(int memberId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $" SELECT Groups.groupId, Groups.name" +
                             $" FROM {MemberGroupsTableName} Groups" +
                             $" JOIN {MemberGroupMembersTableName} Members ON Groups.groupId = Members.groupId" +
                             $" WHERE Members.memberId = @memberId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;

                    return ReadRecords(cmd, ReadMemberGroup);
                }
            }
        }

        public PasswordResetRequest GetPasswordResetRequest(string token)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"SELECT passwordResetRequestId, memberId, created, expires, token " +
                             $" FROM {PasswordResetRequestsTableName} " +
                             $" WHERE token = @token";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@token", SqlDbType.NVarChar).Value = token;

                    return ReadRecord(cmd, ReadPasswordResetRequest);
                }
            }
        }

        public void RemoveMemberFromGroup(int memberId, int groupId)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"DELETE {MemberGroupMembersTableName} WHERE memberId = @memberId AND groupId = @groupId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;
                    cmd.Parameters.Add("@groupId", SqlDbType.Int).Value = groupId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateMemberGroup(int chapterId, string oldName, string newName)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sql = $"UPDATE {MemberGroupsTableName} SET name = @newName WHERE chapterId = @chapterId AND name = @oldName";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@chapterId", SqlDbType.Int).Value = chapterId;
                    cmd.Parameters.Add("@oldName", SqlDbType.NVarChar).Value = oldName;
                    cmd.Parameters.Add("@newName", SqlDbType.NVarChar).Value = newName;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static MemberGroup ReadMemberGroup(SqlDataReader reader)
        {
            return new MemberGroup
            {
                GroupId = reader.GetInt32(reader.GetOrdinal("groupId")),
                Name = reader.GetString(reader.GetOrdinal("name"))
            };
        }

        private static PasswordResetRequest ReadPasswordResetRequest(SqlDataReader reader)
        {
            return new PasswordResetRequest
            {
                Created = reader.GetDateTime(reader.GetOrdinal("created")),
                Expires = reader.GetDateTime(reader.GetOrdinal("expires")),
                MemberId = reader.GetInt32(reader.GetOrdinal("memberId")),
                PasswordResetRequestId = reader.GetInt32(reader.GetOrdinal("passwordResetRequestId")),
                Token = reader.GetString(reader.GetOrdinal("token"))
            };
        }
    }
}
