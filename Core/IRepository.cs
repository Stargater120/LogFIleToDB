using Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public interface IRepository
    {
        Task CreateEntry(string command);
        Task<LogEntry> GetEntryAsync(string query);
        Task<List<LogEntry>> GetEntriesAsync(string query);
    }
}