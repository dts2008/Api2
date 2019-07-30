using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class Tools
    {
        static public T Deserialize<T>(string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
            }

            return default(T);
        }
    }
}
