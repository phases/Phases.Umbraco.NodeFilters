using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phases.Umbraco.NodeFilters.Models.FilterNodes
{
    public class ValuesForFilter
    {
        public string FilteredMainCategory { get; set; }
        public string FilteredCategory { get; set; }
        public string FilteredSubCategory { get; set; }
        public string FilteredValue { get; set; }
        public string FilteredDate { get; set; }
        public string FilteredEndDate { get; set; }
        public string FilteredContentType { get; set; }
    }
}
