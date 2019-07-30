using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public sealed class Api2Result : Dictionary<string, object>
    {
        public const string status_ok = "OK";

        public const string status_error = "ERROR";

        public Dictionary<string, object> data
        {
            get { return this; }
        }

        public Api2Result() { this["cmd"] = "init"; }

        public Api2Result(string c)
        {
            this["cmd"] = c;
            this["time"] = (int)((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            this["status"] = status_ok;
        }

        
    }
}
