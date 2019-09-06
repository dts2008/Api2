using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public enum EventTypes
    {
        none = 0,
        email = 1,
        phone = 2,
        skype = 3,
        telegram = 4,
        whatsapp = 5
    }

    [Table("eventinfo")]
    public class EventInfo : CommonInfo
    {
        public int partnerid { get; set; }

        public int contactid { get; set; }

        public int contactType { get; set; }

        public long eventtime { get; set; }

        public string description { get; set; }
    }
}
