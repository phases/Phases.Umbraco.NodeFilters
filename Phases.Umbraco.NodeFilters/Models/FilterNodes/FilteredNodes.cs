using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phases.Umbraco.NodeFilters.Models.FilterNodes
{
    public class FilteredNodes
    {
        public int NodeId { get; set; }
        public string NodeName { get; set; }
        public string NodeType { get; set; }
        public string NodeUrl { get; set; }
        public string NodeUmbracoUrl { get; set; }
        public string NodeCreatedDate { get; set; }
        public string NodeUpdatedDate { get; set; }
        public string NodeFilterCondition { get; set; }
    }
}
