using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Api2
{
    public class CommonManager<T>: IManager where T: CommonInfo
    {
        public List<T> commonItems = new List<T>();

        public Random sequence = new Random((int)DateTime.UtcNow.Ticks);

        public FieldInfo[] typeFields;

        protected Action<T, T> updateAction = null;

        public CommonManager()
        {
            var t = typeof(T);
            typeFields = t.GetFields();

            Init();
        }

        public virtual void Init()
        {
            //for (int i = 0; i < 30; ++i)
            //{
            //    var item = new UserInfo();

            //    item.id = i + 1;
            //    item.login = $"login{i + 1}";
            //    item.name = $"Name {i + 1}";
            //    item.role = sequence.Next(3) + 1;
            //    item.partners = sequence.Next(50);
            //    item.activity = (int)((DateTimeOffset)DateTime.UtcNow.AddHours(-sequence.Next(72))).ToUnixTimeSeconds();
            //    item.contacts = "";

            //    userItems.Add(item);
            //}
        }

        public bool Delete(int id)
        {
            int index = commonItems.FindIndex(i => i.id == id);

            if (index == -1)
                return false;

            commonItems.RemoveAt(index);
            return true;
        }

        public bool Update(string data, out int id)
        {
            var item = Tools.Deserialize<T>(data);
            id = 0;

            if (item == null) return false;

            if (item.id > 0)
            {
                int index = commonItems.FindIndex(i => i.id == item.id);

                if (index != -1)
                {
                    updateAction?.Invoke(item, commonItems[index]);
                    //if (string.IsNullOrEmpty(item.password)) item.password = commonItems[index].password;
                    commonItems[index] = item;
                }
                else
                    commonItems.Add(item);

                return true;
            }

            item.id = commonItems.Count > 0 ? commonItems.Max(i => i.id) + 1 : 1;
            commonItems.Add(item);

            id = item.id;
            return true;
        }

        public Array Get(int page, int pageSize, out int total_items, string sort_by, bool descending)
        {
            var result = new List<T>();

            var items = string.IsNullOrEmpty(sort_by) ? commonItems : OrderByField(sort_by, descending);

            total_items = items.Count;
            int startIndex = (page - 1) * pageSize;

            for (int i = 0; i < pageSize; ++i)
            {
                if (startIndex + i >= items.Count)
                    break;

                result.Add(items[startIndex + i]);
            }

            return result.ToArray();
        }

        private List<T> OrderByField(string sort_by, bool descending)
        {
            var field = typeFields.FirstOrDefault(f => f.Name == sort_by.ToLower());

            if (field == null) return commonItems;

            if (!descending)
                return commonItems.OrderBy(x => field.GetValue(x)).ToList();

            return commonItems.OrderByDescending(x => field.GetValue(x)).ToList();
        }
    }
}
