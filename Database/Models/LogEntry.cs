using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        public string IPAddress { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Method { get; set; }
        public string URL { get; set; }
        public int StatusCode { get; set; }
        public string ResponseTime { get; set; }
        public string Protocol { get; set; }
    }
}
