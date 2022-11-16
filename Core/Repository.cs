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
                        TimeStamp = _sqlHelper.GetStringNullable(reader, 0),
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
                        TimeStamp = _sqlHelper.GetStringNullable(reader, 0),
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
        
        protected async IAsyncEnumerable<AttributeWithCount> GetAttributeWithCount(string query, string columnName, LogEntriesFilter? filter)
        {
            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();

            if (filter != null)
            {
                query += " HAVING ";
                var filters = AddFiltersAsync(filter, cmd.Parameters).ToList();
                if (filters.Any())
                {
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

            using var reader = cmd.ExecuteReader();
            
            while (await reader.ReadAsync())
            {
                var attributeWithCount = new AttributeWithCount() { 
                    AttributeValue = columnName != "status_code" ? _sqlHelper.GetStringNullable(reader, 0) : reader.GetInt32(0).ToString(),
                    AttributeCount = reader.GetInt32(1)
                };
                yield return attributeWithCount;
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

        private static IEnumerable<string> AddFiltersAsync(LogEntriesFilter filter, SqliteParameterCollection parameters)
        {
            if (filter.TimeRange != null)
            {
                parameters.AddWithValue("begin", filter.TimeRange.Begin);
                parameters.AddWithValue("end", filter.TimeRange.End);
                yield return " TimeStamp >= @begin AND TimeStamp <= @end ";
            }

            if (filter.IPAdresses?.Count > 0)
            {
                IEnumerable<string> ipAdresses = filter.IPAdresses.Select((ipAdress, i) =>
                {
                    parameters.AddWithValue($"ipAdress{i}", filter.IPAdresses[i]);
                    return @$"@ipAdress{i}";
                });
                yield return $" ip_address IN ({string.Join(",", ipAdresses)})";
            }

            if (filter.Methods?.Count > 0)
            {
                IEnumerable<string> methods = filter.Methods.Select((method, i) =>
                {
                    parameters.AddWithValue($"method{i}", filter.Methods);
                    return $@"@method{i}";
                });
                yield return $" method IN ({string.Join(",", methods)})";
            }

            if (filter.StatusCodes?.Count > 0)
            {
                IEnumerable<string> statusCodes = filter.StatusCodes.Select((statusCode, i) =>
                {
                    parameters.AddWithValue($"statusCode{i}", filter.StatusCodes[i]);
                    return $@"@statusCode{i}";
                });
                yield return $" status_code IN ({string.Join(",", statusCodes)})";
            }
        }
    }
}