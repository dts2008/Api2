using System;
using System.Collections.Generic;
using System.IO;
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

        private static Dictionary<Type, Func<FilterType, object, object, bool>> compareMethods = new Dictionary<Type, Func<FilterType, object, object, bool>>();

        static CommonManager()
        {
            compareMethods[typeof(int)] = CompareInt;
            compareMethods[typeof(int)] = CompareLong;
        }

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

        public virtual bool Delete(int id)
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

        public virtual async Task<int> Upload(string fileName, Stream stream, string item)
        {
            await Task<int>.CompletedTask;

            return 0;
        }

        public virtual string Download(int id)
        {
            return string.Empty;
        }

        public Array Get(int page, int pageSize, out int total_items, string sort_by, bool descending, List<FilterItem> filterList)
        {
            List<FilterItem> filters = null;// new List<FilterItem>();

            if (filterList?.Count > 0)
            {
                for (int i = 0; i < filterList.Count; ++i)
                {
                    var field = typeFields.FirstOrDefault(f => f.Name == filterList[i].name);
                    if (field == null) continue;

                    if (filters == null) filters = new List<FilterItem>();

                    filterList[i].field = field;
                    filters.Add(filterList[i]);
                }
            }
            var result = new List<T>();

            var items = string.IsNullOrEmpty(sort_by) ? commonItems : OrderByField(sort_by, descending);

            total_items = items.Count;
            int startIndex = (page - 1) * pageSize;

            int index = -1;

            while (result.Count < pageSize)
            {
                ++index;

                if (startIndex + index >= items.Count)
                    break;

                if (filters?.Count > 0)
                {
                    bool isContinue = false;
                    for (int j = 0; j < filters.Count; ++j)
                    {
                        var fValue = filters[j].field.GetValue(items[startIndex + index]);
                        if (fValue == null) continue;

                        if (!compareMethods.TryGetValue(filters[j].field.FieldType, out var func)) continue;

                            if (!func(filters[j].type, filters[j].value, fValue))
                            {
                                isContinue = true;
                                break;
                            }
                        
                    }

                    if (isContinue) continue;
                }

                result.Add(items[startIndex + index]);
            }

            return result.ToArray();
        }

        #region Private method(s)

        private List<T> OrderByField(string sort_by, bool descending)
        {
            var field = typeFields.FirstOrDefault(f => f.Name == sort_by.ToLower());

            if (field == null) return commonItems;

            if (!descending)
                return commonItems.OrderBy(x => field.GetValue(x)).ToList();

            return commonItems.OrderByDescending(x => field.GetValue(x)).ToList();
        }

        #region Compare Methods

        private static bool CompareInt(FilterType type, object t1, object t2)
        {
                switch (type)
                {
                    case FilterType.Equal: return Tools.ToInt(t1) == Tools.ToInt(t2);
                    case FilterType.MoreOrEqual: return Tools.ToInt(t1) >= Tools.ToInt(t2);
                    case FilterType.LessOrEqual: return Tools.ToInt(t1) <= Tools.ToInt(t2);
                    case FilterType.More: return Tools.ToInt(t1) > Tools.ToInt(t2);
                    case FilterType.Less: return Tools.ToInt(t1) < Tools.ToInt(t2);
                    default:
                        return false;
                }
        }

        private static bool CompareLong(FilterType type, object t1, object t2)
        {
            switch (type)
            {
                case FilterType.Equal: return Tools.ToInt(t1) == Tools.ToInt(t2);
                case FilterType.MoreOrEqual: return Tools.ToInt(t1) >= Tools.ToInt(t2);
                case FilterType.LessOrEqual: return Tools.ToInt(t1) <= Tools.ToInt(t2);
                case FilterType.More: return Tools.ToInt(t1) > Tools.ToInt(t2);
                case FilterType.Less: return Tools.ToInt(t1) < Tools.ToInt(t2);
                default:
                    return false;
            }
        }

        #endregion Compare Methods

        #endregion
    }
}
