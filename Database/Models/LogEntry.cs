using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Models
{
    public class LogEntry
    {
        public string IPAddress { get; set; }
        public string TimeStamp { get; set; }
        public string Method { get; set; }
        public string URL { get; set; }
        public int StatusCode { get; set; }
        public string ResponseTime { get; set; }
        public string Protocol { get; set; }
    }
}
