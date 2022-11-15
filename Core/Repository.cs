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
        protected readonly DBContext _dBContext;
        private readonly SQLHelper _sqlHelper;
        public Repository()
        {
           _dBContext = new DBContext();
           _sqlHelper = new SQLHelper();
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
                        Method = _sqlHelper.GetStringNullable(reader, 2),
                        URL = _sqlHelper.GetStringNullable(reader, 3),
                        StatusCode = reader.GetInt32(4),
                        ResponseTime = _sqlHelper.GetStringNullable(reader, 5),
                        IPAddress = _sqlHelper.GetStringNullable(reader, 6),
                        Protocol = _sqlHelper.GetStringNullable(reader, 7)
                    };
                    return entry;
                }
                throw new Exception("Kein eintrag gefunden");
            }
        }

        protected async IAsyncEnumerable<LogEntry> GetEntriesAsync(SqliteCommand cmd)        {
            
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        LogEntry entry = new LogEntry
                        {
                            Id = reader.GetInt32(0),
                            TimeStamp = reader.GetDateTime(1),
                            Method = _sqlHelper.GetStringNullable(reader, 2),
                            URL = _sqlHelper.GetStringNullable(reader, 3),
                            StatusCode = reader.GetInt32(4),
                            ResponseTime = _sqlHelper.GetStringNullable(reader, 5),
                            IPAddress = _sqlHelper.GetStringNullable(reader, 6),
                            Protocol = _sqlHelper.GetStringNullable(reader, 7)
                        };
                        yield return entry;
                    
                }
                throw new Exception("Kein eintrag gefunden");
            }
        }
    }
}
