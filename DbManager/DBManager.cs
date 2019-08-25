using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class DBManager
    {
        public static Dictionary<string, IManager> managers = new Dictionary<string, IManager>();

        public static DBManager _instance = new DBManager();

        public static DBManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private DBManager()
        {
            managers["userinfo"] = new UserManager();
            managers["partnerinfo"] = new PartnerManager();
            managers["contactinfo"] = new ContactManager();
            managers["partnerfileinfo"] = new PartnerFileManager();
            
        }

        public bool Get(string type, out IManager manager)
        {
            return managers.TryGetValue(type, out manager);
        }
    }
}
