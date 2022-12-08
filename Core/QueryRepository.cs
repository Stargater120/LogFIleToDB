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
        public readonly DisplayedLists _displayedLists;
        private Dictionary<OrderingProperties, string> columnNames = new Dictionary<OrderingProperties, string>() {
            {OrderingProperties.IP, "ip_address" },
            {OrderingProperties.Method, "method" },
            {OrderingProperties.ResponseTime, "response_time" },
            {OrderingProperties.TimeStamp, "time_stamp" },
            {OrderingProperties.Code, "status_code" },
        };

        public QueryRepository(DBContext context, SQLHelper helper, DisplayedLists displayedLists) : base(context, helper)
        {
            _displayedLists = displayedLists;
        }

        public async Task GetAllLogEntriesAsync()
        {
            string query = @"SELECT
                                time_stamp, 
                                method, 
                                url, 
                                status_code, 
                                response_time,
                                ip_address,
                                protocol 
                            FROM log_entry";

            await foreach (var entrys in GetEntriesAsync(null, query))
            {
                DisplayedLists._logEntrys.Add(entrys);
            }
        }

        public async Task<long> GetTotalCountAsync()
        {
            string query = "SELECT COUNT(*) FROM log_entry";            
            return await GetCountAsync(query);
        }
      
        #nullable enable
        public IAsyncEnumerable<AttributeWithCount> GetAttributeValueWithCountAsync(OrderingProperties attribute, LogEntriesFilter? filter)
        {
            string query = $"SELECT {columnNames[attribute]}, COUNT(*) as count FROM log_entry GROUP BY {columnNames[attribute]}";
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
        /// <summary>
        /// delivers max ten options for dropdown of multiselect filters
        /// </summary>
        /// <param name="filterType">used to get the name of the column to read values from</param>
        /// <param name="searchValue">if filter offers textsearch param is used to add user input to query</param>
        /// <param name="offset">used to load more values if reuquested</param>
        /// <returns></returns>
        public IAsyncEnumerable<string> GetOptionsForFilter(OrderingProperties filterType) 
        {
            var columnName = columnNames[filterType];

            string query = $"SELECT DISTINCT {columnName} FROM log_entry";                              

            query += $" ORDER BY {columnName} ASC";            
            
            return GetOptionsForMultiselectAsync(query);
        }
        /// <summary>
        /// Gets the min and max value to use as constraints in DateTimePickers
        /// </summary>
        /// <returns>A value Task/ A TimeRange object holding the earliest and latest Date the 
        /// user can pick when filtering with TimeStamps</returns>
        public async Task<TimeRange> GetTimeRangeForFilterAsync()
        {
            string query = "SELECT MIN(time_stamp), MAX(time_stamp) FROM log_entry;";
            return await GetLimitsforDateTimePicker(query);
        }

        public IAsyncEnumerable<LogFile> GetAllPreviouslyLoadedFiles()
        {
            string query = "SELECT * FROM file";
            return GetAllFilesAsync(query);
        }

    }
}

        
