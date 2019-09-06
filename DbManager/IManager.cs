using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public interface IManager
    {
        bool Delete(UserItem userItem, int id);

        bool Update(UserItem userItem, string json, out int id);

        List<CommonInfo> Get(UserItem userItem, int page, int pageSize, out int total_items, string sort_by, bool descending, List<FilterItem> filterList);

        CommonInfo Get(UserItem userItem, int id);

        CommonInfo Get(string field, object value);

        Task<int> Upload(UserItem userItem, string fileName, Stream stream, string item);

        string Download(UserItem userItem, int id);

        Dictionary<string, List<CommonInfo>> Dependence(UserItem userItem, List<CommonInfo> origin);
    }
}
