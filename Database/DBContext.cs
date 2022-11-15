using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Database
{
    public class DBContext
    {
        public SqliteConnection GetOpenDBConnection()
        {
            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "DB.db");
            var connection = new SqliteConnection($"Data Source= {dbPath};");
            connection.Open();
            return connection;
        }
    }
}
