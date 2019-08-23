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
                if (string.IsNullOrEmpty(data)) return default(T);

                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
            }

            return default(T);
        }

        static public int ToInt(object s)
        {
            try
            {
                if (s == null) return 0;
                return Convert.ToInt32(s);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
