using Database;
using Database.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Repository
    {
        private readonly DBContext _dBContext;
        public Repository()
        {
           _dBContext = new DBContext();
        }

        protected async Task CreateEntry(string command)
        {
            using var connection = _dBContext.GetOpenDBConnection();

            using(var cmd = connection.CreateCommand())
            {
                cmd.CommandText = command;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        protected async Task<LogEntry> GetEntryAsync(string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using(var cmd = connection.CreateCommand())
            {
                cmd.CommandText = query;
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    var entry = new LogEntry
                    {
                        Id = reader.GetInt32(0),
                        TimeStamp = reader.GetDateTime(1),
                        Method = reader.GetString(2),
                        URL = reader.GetString(3),
                        StatusCode = reader.GetInt32(4),
                        ResponseTime = reader.GetString(5),
                        IPAddress = reader.GetString(6),
                        Protocol = reader.GetString(8)
                    };
                    return entry;
                }
                throw new Exception("Kein eintrag gefunden");
            }
        }

        protected async IAsyncEnumerable<LogEntry> GetEntriesAsync(string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = query;
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        LogEntry entry = new LogEntry
                        {
                            Id = reader.GetInt32(0),
                            TimeStamp = reader.GetDateTime(1),
                            Method = reader.GetString(2),
                            URL = reader.GetString(3),
                            StatusCode = reader.GetInt32(4),
                            ResponseTime = reader.GetString(5),
                            IPAddress = reader.GetString(6),
                            Protocol = reader.GetString(8)
                        };
                        yield return entry;
                    }
                }
                throw new Exception("Kein eintrag gefunden");
            }
        }
    }
}
