using Microsoft.AspNetCore.Mvc;
using Phases.Umbraco.NodeFilters.Services.Interfaces.FilterNodes;
using Phases.Umbraco.NodeFilters.Models.FilterNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Web.Common.Controllers;

namespace Phases.Umbraco.NodeFilters.Controllers.FilterNodes
{
    [Area("filternodes")]
    public class FilterNodesApiController : UmbracoAuthorizedController
    {
        private readonly IFilterNodeServices _filterNodeServices;
        public FilterNodesApiController(IFilterNodeServices filterNodeServices)
        {
            _filterNodeServices = filterNodeServices;
        }

        [HttpGet]
        public JsonResult GetAllCategories()
        {
            var nameList = _filterNodeServices.GetAllUmbracoNodeProperties();


            var result = new
            {
                Categories = nameList
            };

            return new JsonResult(new
            {
                Data = result

            });
        }

        [HttpGet]
        public JsonResult GetSubcategories(string category)
        {
            var propertyList = _filterNodeServices.GetAllProperties(category);
            return new JsonResult(new
            {
                Data = propertyList

            });
        }

        [HttpGet]
        public JsonResult GetValuesFromProperty(string property)
        {
            var propertyList = _filterNodeServices.GetPropertyValues(property);
            return new JsonResult(new
            {
                Data = propertyList

            });
        }

        [HttpPost]
        public ActionResult FilterNodes([FromBody] List<ValuesForFilter> filteredDataList)
        {

            var data = _filterNodeServices.FilterNodes(filteredDataList);
            return new JsonResult(new
            {
                Data = data

            });
        }
    }
}
