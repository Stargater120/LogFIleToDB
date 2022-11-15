using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Database
{
    public class SQLHelper
    {
        public string GetStringNullable<T>(T reader, int colIndex) where T : DbDataReader
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }

            return string.Empty;
        }
    }
}
