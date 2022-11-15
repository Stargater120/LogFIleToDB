using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Database.Models;

namespace Core
{   /// Summary
    /// Class to use for Read Actions 
    public class QueryRepository : Repository
    {        
        public IAsyncEnumerable<LogEntry> GetAllLogEntriesAsync()
        {
            string query = "SELECT * FROM log_entry";
            return GetEntriesAsync(query);
        }

        public IAsyncEnumerable<LogEntry> GetAllLogEntriesInTimeRangeAsync()
    }
}
