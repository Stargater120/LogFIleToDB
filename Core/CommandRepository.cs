using Database;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Core
{
    public class CommandRepository : Repository
    {
        public CommandRepository(DBContext context, SQLHelper helper): base(context, helper)
        {            
        }
        public async Task CreateLogEntryCommand(List<LogEntry> logEntries)
        {
            string command = "INSERT INTO log_entry(time_stamp, method, url, status_code, response_time, ip_address, protocol) VALUES";
            foreach (LogEntry logEntry in logEntries)
            {
                string newValue = $"({logEntry.TimeStamp}, {logEntry.Method}, {logEntry.URL}, {logEntry.StatusCode}, {logEntry.ResponseTime}, {logEntry.IPAddress}, {logEntry.Protocol}),";
                command += newValue;
                break;
            }

            await CreateEntry(command);
        }

        public async Task CreateLogEntrys(string filePath)
        {
            var file = File.ReadAllText(filePath);
            string[] seperater = new string[] { "\r", "\n" };
            var lines = file.Split(seperater, StringSplitOptions.RemoveEmptyEntries);
            var logEntrys = new List<LogEntry>();
            foreach (string logLine in lines)
            {
                var entry = new LogEntry();
                entry.IPAddress = logLine.Substring(0, logLine.IndexOf(" - -") - 1);
                string date = logLine.Substring(logLine.IndexOf("[") + 1, (logLine.IndexOf("]")) - (logLine.IndexOf("[") + 1));
                entry.TimeStamp = DateTime.Parse(date);
                entry.Method = logLine.Substring(logLine.IndexOf("]") + 3, logLine.IndexOf("/") - (logLine.IndexOf("]") + 3));
                entry.URL = logLine.Substring(logLine.IndexOf("/"), (logLine.IndexOf("HTTP") - 2) - (logLine.IndexOf("/") - 1));
                entry.StatusCode = int.Parse(logLine.Substring(logLine.LastIndexOf((char)34) + 2, 3));
                entry.ResponseTime = logLine.Substring(logLine.LastIndexOf((char)34) + 5);
                entry.Protocol = logLine.Substring(logLine.IndexOf("HTTP"), 8);
                logEntrys.Add(entry);
            }

            await CreateLogEntryCommand(logEntrys);
        }
    }
}
