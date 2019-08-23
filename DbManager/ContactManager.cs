using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class ContactManager : CommonManager<ContactInfo>
    {
        public override void Init()
        {
            for (int i = 0; i < 100; ++i)
            {
                var item = new ContactInfo();

                item.id = i + 1;
                item.name = $"FIO {i + 1}";
                item.email = $"test_{i + 1}@gmail.com";
                item.phone = $"+380-50-{i + 1}{i + 1}{i + 1}";
                item.skype = "";
                item.telegram = "";
                item.whatsapp = "";
                item.comment = "";
                item.partnerid = sequence.Next(30) + 1;

                commonItems.Add(item);
            }
        }
    }
}
