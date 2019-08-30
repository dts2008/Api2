using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class UserManager : CommonDBManager<UserInfo>
    {
        public override void Init()
        {
        }

        public override bool UpdateItem(UserInfo newValue, UserInfo oldValue)
        {
            if (string.IsNullOrEmpty(newValue.password)) newValue.password = oldValue.password;

            // check rigt
            return true;
        }

        public bool UpdateActivity(UserInfo user)
        {
            return DBCommand((IDbConnection db) =>
            {
                db.Execute($"update {typeName} set activity = {((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()} where id = {user.id}");
            }
            );
        }
    }
}
