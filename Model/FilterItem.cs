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
    }
    public class FilterItem
    {
        public string name;

        public string value;

        public FilterType type;

        public FieldInfo field;
    }
}
