using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phases.Umbraco.NodeFilters.Models.FilterNodes
{
    public class CustomPropertyInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string DataType { get; set; }
    }

    public class FilteredData
    {
        public List<List<CustomPropertyInfo>> FilteredCategories { get; set; }
        public List<List<CustomPropertyInfo>> FilteredSubCategories { get; set; }
        public List<List<CustomPropertyInfo>> FilteredValues { get; set; }
    }
}
