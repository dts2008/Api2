using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    [Table("userinfo")]
    public class UserInfo : CommonInfo
    {
        public string name { get; set; }

        public string login { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public string contacts { get; set; }

        public int role { get; set; }

        public int activity { get; set; }
    }
}
