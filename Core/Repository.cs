using Database;
using Database.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public class Repository
    {
        protected readonly DBContext _dBContext;
        private readonly SQLHelper _sqlHelper;
        public Repository(DBContext context, SQLHelper helper)
        {
            _dBContext = context;
            _sqlHelper = helper;
        }

        protected async Task CreateEntry(List<string> commands)
        {
            int counter = 0;
            try
            {
                using (var cmd = _connection.CreateCommand())
                {
                    _connection.Open();
                    foreach (string command in commands)
                    {
                        cmd.CommandText = command;
                        cmd.ExecuteNonQuery();
                        counter++;
                    }
                    _connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        protected async Task<LogEntry> GetEntryAsync(string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using(var cmd = connection.CreateCommand())
            {
                cmd.CommandText = query;
                using var reader = cmd.ExecuteReader();
                while (reader.HasRows)
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
                throw new Exception("Kein Eintrag gefunden");
            }
        }

        protected async IAsyncEnumerable<LogEntry> GetEntriesAsync(SqliteCommand cmd){
            
                using var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
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
                throw new Exception("Kein Eintrag gefunden");
            }
        }

        protected async Task<long> GetCountAsync(SqliteCommand cmd)
        {
            #nullable enable
            object? result = cmd.ExecuteScalarAsync();
            return (long)result;
        }

    }
}
