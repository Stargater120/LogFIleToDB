using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;
using Core.Models;
using Database;
using Database.Models;
using Microsoft.Data.Sqlite;

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
            using var connection = _dBContext.GetOpenDBConnection();
            int counter = 0;
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    foreach (string command in commands)
                    {
                        cmd.CommandText = command;
                        cmd.ExecuteNonQuery();
                        counter++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected async Task<long?> CreateFileEntryAndDeliverId(string command, string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();

            using var transaction = connection.BeginTransaction();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = command;
            var rowsAffected = cmd.ExecuteNonQuery();

            using var getIDCmd = connection.CreateCommand();
            getIDCmd.CommandText = query;
            var result = await getIDCmd.ExecuteScalarAsync();

            if (result != null)
            {
                transaction.Commit();
            }
            else
            {
                transaction.Rollback();
            }

            return (long?)result;
        }

        protected async Task<LogEntry> GetEntryAsync(string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = query;
                using var reader = cmd.ExecuteReader();
                while (reader.HasRows)
                {
                    var entry = new LogEntry
                    {
                        TimeStamp = reader.GetDateTime(0),
                        Method = _sqlHelper.GetStringNullable(reader, 1),
                        URL = _sqlHelper.GetStringNullable(reader, 2),
                        StatusCode = reader.GetInt32(3),
                        ResponseTime = _sqlHelper.GetStringNullable(reader, 4),
                        IPAddress = _sqlHelper.GetStringNullable(reader, 5),
                        Protocol = _sqlHelper.GetStringNullable(reader, 6)
                    };
                    return entry;
                }

                throw new Exception("Kein Eintrag gefunden");
            }
        }

#nullable enable
        protected async IAsyncEnumerable<LogEntry> GetEntriesAsync(LogEntriesFilter? filter, string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();

            if (filter != null)
            {
                var filters = AddFiltersAsync(filter, cmd.Parameters).ToList();
                if (filters.Any())
                {
                    query += " WHERE ";
                    query += string.Join(" AND ", filters);
                }

                if (filter.OrderBy != null)
                {
                    query += await GetOrderProperty(filter.OrderBy.Value);
                }

                if (filter.Order != null)
                {
                    query += GetOrder(filter.Order.Value);
                }
            }

            cmd.CommandText = query;

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    LogEntry entry = new LogEntry
                    {
                        TimeStamp = reader.GetDateTime(0),
                        Method = _sqlHelper.GetStringNullable(reader, 1),
                        URL = _sqlHelper.GetStringNullable(reader, 2),
                        StatusCode = reader.GetInt32(3),
                        ResponseTime = _sqlHelper.GetStringNullable(reader, 4),
                        IPAddress = _sqlHelper.GetStringNullable(reader, 5),
                        Protocol = _sqlHelper.GetStringNullable(reader, 6)
                    };
                    yield return entry;
                }
            }
            else
            {
                throw new Exception("Keine Einträge gefunden.");
            }
        }

        protected async Task<long> GetCountAsync(string query)
        {
#nullable enable
            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            object? result = cmd.ExecuteScalarAsync();
            return (long)result;
        }

        protected async IAsyncEnumerable<AttributeWithCount> GetAttributeWithCount(string query, string columnName,
            LogEntriesFilter? filter)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();

            if (filter != null)
            {
                var filters = AddFiltersAsync(filter, cmd.Parameters).ToList();
                if (filters.Any())
                {
                    query += " HAVING ";
                    query += string.Join(" AND ", filters);
                }
            }

            query += " ORDER BY count ASC ";


            cmd.CommandText = query;

            using var reader = cmd.ExecuteReader();

            while (await reader.ReadAsync())
            {
                var attributeWithCount = new AttributeWithCount()
                {
                    AttributeValue = columnName != "status_code"
                        ? _sqlHelper.GetStringNullable(reader, 0)
                        : reader.GetInt32(0).ToString(),
                    AttributeCount = reader.GetInt32(1)
                };
                yield return attributeWithCount;
            }
        }

        #region values for filter

#nullable enable
        protected async IAsyncEnumerable<string> GetOptionsForMultiselectAsync(string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();


            cmd.CommandText = query;

            using var reader = cmd.ExecuteReader();

            while (await reader.ReadAsync())
            {
                yield return _sqlHelper.GetStringNullable(reader, 0);
            }
        }
#nullable disable

        protected async Task<TimeRange> GetLimitsforDateTimePicker(string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();

            cmd.CommandText = query;

            using var reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    var timeRange = new TimeRange()
                    {
                        Begin = reader.GetDateTime(0),
                        End = reader.GetDateTime(1)
                    };
                    return timeRange;
                }
            }

            throw new Exception("Keine Einträge gefunden");
        }

        #endregion

        protected async IAsyncEnumerable<LogFile> GetAllFilesAsync(string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();

            cmd.CommandText = query;

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var logFile = new LogFile()
                {
                    FileName = _sqlHelper.GetStringNullable(reader, 1),
                    LoadenOn = reader.GetDateTime(2),
                    Entries = reader.GetInt32(3),
                };
                yield return logFile;
            }
        }

        protected async Task<LogFile> GetLogFile(string query)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            using var reader = await cmd.ExecuteReaderAsync();
            try
            {
                await reader.ReadAsync();
                return new LogFile
                {
                    FileName = reader.GetString(1),
                    LoadenOn = reader.GetDateTime(2),
                    Entries = reader.GetInt32(3)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<string> GetOrderProperty(OrderingProperties property)
        {
            string orderBy;
            switch (property)
            {
                case OrderingProperties.ResponseTime:
                    orderBy = " ORDER BY response_time ";
                    break;

                case OrderingProperties.Method:
                    orderBy = " ORDER BY method ";
                    break;

                default:
                    orderBy = " ORDER BY time_stamp ";
                    break;
            }

            return orderBy;
        }

        private static string GetOrder(Order order)
        {
            return order == Order.Descending ? " DESC" : " ASC";
        }

        private static IEnumerable<string> AddFiltersAsync(LogEntriesFilter filter,
            SqliteParameterCollection parameters)
        {
            if (filter.Begin.HasValue)
            {
                parameters.AddWithValue("begin", filter.Begin.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                yield return " time_stamp >= @begin ";
            }

            if (filter.End.HasValue)
            {
                parameters.AddWithValue("end", filter.End.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                yield return " time_stamp <= @end ";
            }

            if (!string.IsNullOrWhiteSpace(filter.IPAdresses))
            {
                parameters.AddWithValue($"ipAdress0", filter.IPAdresses);
                yield return $" ip_address like (\"{filter.IPAdresses}\")";
            }

            if (!string.IsNullOrWhiteSpace(filter.Method))
            {
                parameters.AddWithValue($"method0", filter.Method);
                yield return $" method like (\"{filter.Method}\")";
            }

            if (filter.StatusCode != null)
            {
                parameters.AddWithValue($"statusCode0", filter.StatusCode);
                yield return $" status_code = {filter.StatusCode}";
            }
        }
    }
}