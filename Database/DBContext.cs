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
            // DB file pfad für die Veröffentliche version
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "DB.db");

            //var baseDirectoryPathe = System.AppDomain.CurrentDomain.BaseDirectory;
            //var dbPath = Path.Combine(Path.GetFullPath(Path.Combine(baseDirectoryPathe, "..", "..", "..")), "DB.db");
            var connection = new SqliteConnection($"Data Source= {dbPath};");
            connection.Open();
            return connection;
        }       
    }
}
