using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class PartnerManager : CommonDBManager<PartnerInfo>
    {
        public override void Init()
        {
            //for (int i = 0; i < 30; ++i)
            //{
            //    var item = new PartnerInfo();

            //    item.id = i + 1;
            //    item.name = $"Name {i + 1}";
            //    item.status = sequence.Next(5);
            //    item.website = $"www.site{i + 1}.com";
            //    item.added = ((DateTimeOffset)DateTime.UtcNow.AddHours(-sequence.Next(720))).ToUnixTimeSeconds();
            //    item.description = "";
            //    item.manager = sequence.Next(3) + 1;

            //    item.clientType = sequence.Next(3);
            //    item.currency = sequence.Next(3);

            //    commonItems.Add(item);
            //}
        }

        public override bool UpdateItem(UserItem userItem, PartnerInfo newValue, PartnerInfo oldValue)
        {
            //if (string.IsNullOrEmpty(newValue.password)) newValue.password = oldValue.password;

            // if not admin check partner_id

            return true;
        }

        public override bool InsertItem(UserItem userItem, PartnerInfo newValue)
        {
            newValue.added = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            newValue.manager = userItem.uid;

            return true;
        }

        public override Dictionary<string, List<CommonInfo>> Dependence(UserItem userItem, List<CommonInfo> origin)
        {
            var result = new Dictionary<string, List<CommonInfo>>();

            var managers = new List<int>();
            
            DBManager.Instance.Get(UserManager.typeName, out var users);

            foreach (var o in origin)
            {
                var partner = o as PartnerInfo;
                if (partner == null) continue;

                if (managers.FindIndex(k => k == partner.manager) != -1)
                    continue;

                managers.Add(partner.manager);
            }

            if (managers.Count > 0)
            {
                var filter = new FilterItem();
                filter.name = "id";
                filter.type = FilterType.In;
                filter.value = $"({String.Join(",", managers)})";

                var partners = users.Get(userItem, 0, -1, out int total_items, string.Empty, false, new List<FilterItem>() { filter });
                result[UserManager.typeName] = partners;
            }

            return result;
        }
    }
}
