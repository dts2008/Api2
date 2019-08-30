using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public interface IManager
    {
        bool Delete(int id);

        bool Update(string json, out int id);

        List<CommonInfo> Get(int page, int pageSize, out int total_items, string sort_by, bool descending, List<FilterItem> filterList);

        CommonInfo Get(int id);

        CommonInfo Get(string field, object value);

        Task<int> Upload(string fileName, Stream stream, string item);

        string Download(int id);

        Dictionary<string, List<CommonInfo>> Dependence(List<CommonInfo> origin);

        Type ItemType();
    }
}
