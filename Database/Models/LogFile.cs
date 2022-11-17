using System;

namespace Database.Models
{
    public class LogFile
    {
        public string FileName { get; set; }
        public DateTime LoadenOn { get; set; }
        public int Entries { get; set; }
    }
}
