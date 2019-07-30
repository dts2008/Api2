using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public interface IManager
    {
        bool Delete(int id);

        bool Update(string json, out int id);

        Array Get(int page, int pageSize, out int total_items, string sort_by, bool descending);
    }
}
