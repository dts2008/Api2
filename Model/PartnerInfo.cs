using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    [Table("partnerinfo")]
    public class PartnerInfo : CommonInfo
    {
        public string name { get; set; }

        public string website { get; set; }

        public int manager { get; set; }

        public long added { get; set; }

        public int status { get; set; }

        public string description { get; set; }

        public int clientType { get; set; }

        public int currency { get; set; }
    }
}
