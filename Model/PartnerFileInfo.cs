using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    [Table("partnerfileinfo")]
    public class PartnerFileInfo : CommonInfo
    {
        public string description { get; set; }

        public string name { get; set; }

        public long size { get; set; }

        public long added { get; set; }

        public int partnerId { get; set; }

        public string fileToken { get; set; }
    }
}
