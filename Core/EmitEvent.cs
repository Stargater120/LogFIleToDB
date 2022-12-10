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
}
