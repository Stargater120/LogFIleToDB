﻿using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
