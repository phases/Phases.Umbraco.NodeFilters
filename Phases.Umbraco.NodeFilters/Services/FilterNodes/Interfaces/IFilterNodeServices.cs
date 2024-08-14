using Phases.Umbraco.NodeFilters.Models.FilterNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phases.Umbraco.NodeFilters.Services.Interfaces.FilterNodes
{
    public interface IFilterNodeServices
    {
        List<CustomPropertyInfo> GetAllUmbracoNodeProperties();
        List<CustomPropertyInfo> GetAllProperties(string contentTypeId);
        List<CustomPropertyInfo> GetPropertyValues(string dataTypeId);
        List<FilteredNodes> FilterNodes(List<ValuesForFilter> filteredDataList);
    }
}
