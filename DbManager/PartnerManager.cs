using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class PartnerManager : CommonManager<PartnerInfo>
    {
        public override void Init()
        {
            for (int i = 0; i < 30; ++i)
            {
                var item = new PartnerInfo();

                item.id = i + 1;
                item.name = $"Name {i + 1}";
                item.status = sequence.Next(5);
                item.website = $"www.site{i + 1}.com";
                item.added = ((DateTimeOffset)DateTime.UtcNow.AddHours(-sequence.Next(720))).ToUnixTimeSeconds();
                item.description = "";
                item.manager = sequence.Next(3) + 1;

                item.clientType = sequence.Next(3);
                item.currency = sequence.Next(3);

                commonItems.Add(item);
            }
        }

        private void Update(UserInfo newValue, UserInfo oldValue)
        {
            if (string.IsNullOrEmpty(newValue.password)) newValue.password = oldValue.password;
        }
    }
}
