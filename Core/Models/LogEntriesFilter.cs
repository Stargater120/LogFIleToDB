using System.Collections.Generic;
using Core.Enums;

namespace Core.Models
{
    public class LogEntriesFilter
    {
        #nullable enable
        public TimeRange? TimeRange { get; set; }
        public List<string>? IPAdresses { get; set; }
        public List<string>? Methods { get; set; }
        public List<int>? StatusCodes { get; set; }
        public OrderingProperties? OrderBy { get; set; }
        public Order? Order { get; set; }

    }
}
