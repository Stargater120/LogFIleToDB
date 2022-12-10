using Database;
using Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
            List<string> commands = new List<string>();
            int counter = 0;
            foreach (var logEntry in logEntries)
            {
                string newValue = $"(\"{logEntry.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\", \"{logEntry.Method}\", \"{logEntry.URL}\", {logEntry.StatusCode}, \"{logEntry.ResponseTime}\", \"{logEntry.IPAddress}\", \"{logEntry.Protocol}\"),";
                command += newValue;
                if(counter % 100 == 0 && counter > 1)
                {
                    command = command.Substring(0, command.Length - 1);
                    command += ";";
                    commands.Add(command);
                    command = "INSERT INTO log_entry(time_stamp, method, url, status_code, response_time, ip_address, protocol) VALUES";
                }
                counter++;
            }
            command = command.Substring(0, command.Length - 1);
            command += ";";
            commands.Add(command);

            await CreateEntry(commands);
        }

        public async Task CreateLogEntrys(string filePath, string fileName)
        {
            await CheckIfFileNameAlreadyExcist(fileName);
            var file = File.ReadAllText(filePath);
            string[] seperater = new string[] { "\r", "\n" };
            var lines = file.Split(seperater, StringSplitOptions.RemoveEmptyEntries);
            var logEntrys = new List<LogEntry>();
            foreach (string logLine in lines)
            {
                var entry = new LogEntry();
                entry.IPAddress = logLine.Substring(0, logLine.IndexOf(" - -"));
                string date = logLine.Substring(logLine.IndexOf("[") + 1, (logLine.IndexOf("]")) - (logLine.IndexOf("[") + 1));
                entry.TimeStamp = DateTime.Parse(date); //DateTime.Parse(date.Replace("/", "-"));
                entry.Method = logLine.Substring(logLine.IndexOf("]") + 3, logLine.IndexOf("/") - (logLine.IndexOf("]") + 4));
                entry.URL = logLine.Substring(logLine.IndexOf("/"), (logLine.IndexOf("HTTP") - 2) - (logLine.IndexOf("/") - 1));
                entry.StatusCode = int.Parse(logLine.Substring(logLine.LastIndexOf((char)34) + 2, 3));
                entry.ResponseTime = logLine.Substring(logLine.LastIndexOf((char)34) + 6);
                entry.Protocol = logLine.Substring(logLine.IndexOf("HTTP"), 8);
                logEntrys.Add(entry);
            }

            try
            {
                await CreateFileEntryGetIdAsync(fileName, logEntrys.Count);
                await CreateLogEntryCommand(logEntrys);
            }
            catch (Exception)
            {

                throw new Exception("Daten konnten nicht hinzugefügt werden");
            }
        }

        public async Task<long?> CreateFileEntryGetIdAsync(string fileName, int entries)
        {
            string command = @$"INSERT INTO file(file_name, loaded_on, entries) 
                                VALUES
                                ('{fileName}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {entries});";

            string query = "SELECT last_insert_rowid();";

            return await CreateFileEntryAndDeliverId(command, query);
        }

        private async Task CheckIfFileNameAlreadyExcist(string fileName)
        {
            string query = @$"SELECT * FROM file WHERE file_name like('{fileName}')";
            var result = await GetLogFile(query);
            if (result != null)
            {
                throw new Exception("Datei wurde schon ein mal Hinzugefügt");
            }
        }
    }
}
