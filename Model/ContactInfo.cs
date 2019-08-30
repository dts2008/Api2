using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    [Table("contactinfo")]
    public class ContactInfo : CommonInfo
    {
        public string name { get; set; }

        public int partnerid { get; set; }

        public string email { get; set; }

        public string phone { get; set; }

        public string skype { get; set; }

        public string telegram { get; set; }

        public string whatsapp { get; set; }

        public string comment { get; set; }

    }
}
