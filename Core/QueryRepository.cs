using System.Collections.Generic;
using Database.Models;
using Core.Enums;
using Core.Models;
using Microsoft.Data.Sqlite;
using System.Linq;
using Database;
using System.Threading.Tasks;

namespace Core { 
    public class QueryRepository : Repository
    {
        public QueryRepository(DBContext context, SQLHelper helper) : base(context, helper)
        {
        }

        public IAsyncEnumerable<LogEntry> GetAllLogEntriesAsync()
        {
            string query = "SELECT * FROM log_entry";
            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            return GetEntriesAsync(cmd);
        }

        public async Task<long> GetTotalCount()
        {
            string query = "SELECT COUNT(*) FROM log_entry";
            using var cmd = new SqliteCommand();
            cmd.CommandText = query;
            return await GetCountAsync(cmd);
        }


        public IAsyncEnumerable<LogEntry> GetFilteredLogEntriesAsync(LogEntriesFilter filter)
        {
            string query = @"SELECT 
                                time_stamp, 
                                method, 
                                url, 
                                status_code, 
                                response_time,
                                ip_adress,
                                protocol
                                FROM log_entry WHERE ";

            using var connection = _dBContext.GetOpenDBConnection();
            using var cmd = connection.CreateCommand();

            var filters = AddFiltersAsync(filter, cmd.Parameters).ToList();
            if (filters.Any())
            {
                query += string.Join(" AND ", filters);
            }

            query += GetOrderProperty(filter.OrderBy);
            query += GetOrder(filter.Order);

            cmd.CommandText = query;
            return GetEntriesAsync(cmd);
        }

        private Dictionary<OrderingProperties, string> propertyNames = new Dictionary<OrderingProperties, string>() {
            {OrderingProperties.IP, "ip_adress" },
            {OrderingProperties.Method, "method" },
            {OrderingProperties.ResponseTime, "response_time" },
            {OrderingProperties.TimeStamp, "time_stamp" },
        };

        private static IEnumerable<string> GetOrderProperty(OrderingProperties property)
        {
            switch (property)
            {
                case OrderingProperties.ResponseTime:
                    yield return " ORDER BY response_time ";
                    break;
                case OrderingProperties.Method:
                    yield return " ORDER BY method ";
                    break;
                default:
                    yield return " ORDER BY time_stamp ";
                    break;
            }
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
                IEnumerable<string> ipAdresses = filter.IPAdresses.Select((ipAdress, i) => {
                    parameters.AddWithValue($"ipAdress{i}", filter.IPAdresses[i]);
                    return  @$"@ipAdress{i}";
                });
                yield return $" ip_adress IN ({string.Join(",", ipAdresses)})";
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
                IEnumerable<string> statusCodes = filter.StatusCodes.Select((statusCode, i) => {
                    parameters.AddWithValue($"statusCode{i}", filter.StatusCodes[i]);
                    return $@"@statusCode{i}";
                });
                yield return $" status_code IN ({string.Join(",", statusCodes)})";
            }

            
        }
    }
}

