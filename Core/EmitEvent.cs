using Core.Models;
using System;


namespace Core
{
    public class EmitEvent : EventArgs
    {
        public LogEntriesFilter logEntries;
        public string IPAddress;

        public EmitEvent()
        {
            logEntries = new LogEntriesFilter();
        }
    }

    public class EmitDateTime : EventArgs
    {
        public DateTime selectedTime { get; set; }
        public bool invalidDate { get; set; }
    }
}