using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Services.ServicesAbstractions;

namespace TeamCapacityBalancing.Services.Postgres_connection
{
    public class QueriesForDataBase : IDataProvider
    {
        private const string getUsersQuery = "SELECT * FROM public.\"cwd_user\"";
        private const string getStoriesByEpicQuery = $"SELECT * FROM jiraissue AS ji JOIN issuelink AS il ON ji.id = il.destination WHERE il.linktype = 10201 and il.source = ";

        List<IssueData> IDataProvider.GetAllEpics(string teamLeaderUsername)
        {
            throw new NotImplementedException();
        }

        List<IssueData> IDataProvider.GetAllStories(string teamLeaderUsername)
        {
            throw new NotImplementedException();
        }

        List<User> IDataProvider.GetAllUsers()
        {
            List<User> users = new List<User>();

            try
            {
                using (var connection = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString()))
                {
                    connection.Open();

                    var cmd = new NpgsqlCommand(getUsersQuery, connection);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string username = reader.GetString(reader.GetOrdinal("user_name"));
                        string displayName = reader.GetString(reader.GetOrdinal("display_name"));
                        int id = reader.GetInt32(reader.GetOrdinal("id"));

                        users.Add(new User(username, displayName, id));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return users;
        }

        List<IssueData> IDataProvider.GetStoriesByEpic(string epicId)
        {
            List<IssueData> stories = new List<IssueData>();

            try
            {
                using (var connection = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString()))
                {
                    connection.Open();

                    var cmd = new NpgsqlCommand(getStoriesByEpicQuery + epicId, connection);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        string name = reader.GetString(reader.GetOrdinal("summary"));

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return stories;
        }
    }
}
