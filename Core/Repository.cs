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
        private readonly SqliteConnection _connection;

        public Repository()
        {
            DBContext dBContext = new DBContext();
            _connection = dBContext.GetDBConnection();
        }

        protected async Task CreateEntry(string command)
        {
            

            using(var cmd = _connection.CreateCommand())
            {
                _connection.Open();
                cmd.CommandText = command;
                await cmd.ExecuteNonQueryAsync();
                _connection.Close();
            }
        }

        protected async Task<LogEntry> GetEntryAsync(string query)
        {
            using(var cmd = _connection.CreateCommand())
            {
                _connection.Open();
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
                    _connection.Close();
                    return entry;
                }
                _connection.Close();
                throw new Exception("Kein eintrag gefunden");
            }
        }

        protected async Task<List<LogEntry>> GetEntriesAsync(string query)
        {
            using (var cmd = _connection.CreateCommand())
            {
                var entries = new List<LogEntry>();
                _connection.Open();
                cmd.CommandText = query;
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        entries.Add(new LogEntry
                        {
                            Id = reader.GetInt32(0),
                            TimeStamp = reader.GetDateTime(1),
                            Method = reader.GetString(2),
                            URL = reader.GetString(3),
                            StatusCode = reader.GetInt32(4),
                            ResponseTime = reader.GetString(5),
                            IPAddress = reader.GetString(6),
                            Protocol = reader.GetString(8)
                        });
                    }
                    
                    _connection.Close();
                    return entries;
                }
                _connection.Close();
                throw new Exception("Kein eintrag gefunden");
            }
        }
    }
}
