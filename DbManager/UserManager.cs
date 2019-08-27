using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class UserManager : CommonManager<UserInfo>
    {
        public override void Init()
        {
            for (int i = 0; i < 30; ++i)
            {
                var item = new UserInfo();

                item.id = i + 1;
                item.login = $"login{i + 1}";
                item.password = Tools.GetMD5("123");
                item.name = $"Name {i + 1}";
                item.role = sequence.Next(3) + 1;
                item.partners = sequence.Next(50);
                item.activity = (int)((DateTimeOffset)DateTime.UtcNow.AddHours(-sequence.Next(72))).ToUnixTimeSeconds();
                item.contacts = "";

                commonItems.Add(item);
            }
        }

        private void Update(UserInfo newValue, UserInfo oldValue)
        {
            if (string.IsNullOrEmpty(newValue.password)) newValue.password = oldValue.password;
        }
    }
}
