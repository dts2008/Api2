using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Api2
{
    public enum FilterType
    {
        Equal = 0,
        MoreOrEqual = 1,
        LessOrEqual = 2,
        More = 3,
        Less = 4,
        In = 5
    }
    public class FilterItem
    {
        public string name { get; set; }

        public string value { get; set; }

        public FilterType type { get; set; }

        public FieldInfo field { get; set; }
    }
}
