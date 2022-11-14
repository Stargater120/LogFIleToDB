using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Database
{
    public class DBContext
    {
        public SqliteConnection GetDBConnection()
        {
            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "DB.db");
            return new SqliteConnection($"Data Source= {dbPath};");
        }
    }
}
