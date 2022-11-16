using System.Collections.Generic;
using Database.Models;
using Core.Models;
using Database;
using System.Threading.Tasks;
using Core.Enums;

namespace Core
{
    public class QueryRepository : Repository
    {
        
        private Dictionary<OrderingProperties, string> columnNames = new Dictionary<OrderingProperties, string>() {
            {OrderingProperties.IP, "ip_address" },
            {OrderingProperties.Method, "method" },
            {OrderingProperties.ResponseTime, "response_time" },
            {OrderingProperties.TimeStamp, "time_stamp" },
            {OrderingProperties.Code, "status_code" },
        };

        public QueryRepository(DBContext context, SQLHelper helper) : base(context, helper)
        {
        }

        public IAsyncEnumerable<LogEntry> GetAllLogEntriesAsync()
        {
            string query = "SELECT * FROM log_entry";           

            return GetEntriesAsync(null, query);
        }

        public async Task<long> GetTotalCount()
        {
            string query = "SELECT COUNT(*) FROM log_entry";            
            return await GetCountAsync(query);
        }
      
        #nullable enable
        public IAsyncEnumerable<AttributeWithCount> GetAttributeValueWithCountAsync(OrderingProperties attribute, LogEntriesFilter? filter)
        {
            string query = $"SELECT {columnNames[attribute]}, COUNT(*) FROM log_entry GROUP BY {columnNames[attribute]}";
            return GetAttributeWithCount(query, columnNames[attribute], filter);
        }

        public IAsyncEnumerable<LogEntry> GetFilteredLogEntriesAsync(LogEntriesFilter filter)
        {
            string query = @"SELECT 
                                time_stamp, 
                                method, 
                                url, 
                                status_code, 
                                response_time,
                                ip_address,
                                protocol
                                FROM log_entry WHERE ";

            return GetEntriesAsync(filter, query);
        }
    }
}

        
