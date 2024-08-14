
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;
using Umbraco.Cms.Core.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Lucene.Net.Queries.Function.ValueSources.MultiFunction;
using Phases.Umbraco.NodeFilters.Models.FilterNodes;
using Umbraco.Cms.Core.PropertyEditors;
using Phases.Umbraco.NodeFilters.Services.Interfaces.FilterNodes;

namespace Phases.Umbraco.NodeFilters.Services.FilterNodes
{
    public class FilterNodeServices : IFilterNodeServices
    {
        private readonly ILocalizationService _localizationService;
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IUmbracoHelperAccessor _helperAccessor;
        private readonly UmbracoHelper _helper;
        public FilterNodeServices(ILocalizationService localizationService, IContentService contentService,
            IContentTypeService contentTypeService, IDataTypeService dataTypeService, IUmbracoHelperAccessor umbracoHelperAccessor
            )
        {
            _localizationService = localizationService;
            _contentService = contentService;
            _contentTypeService = contentTypeService;
            _dataTypeService = dataTypeService;
            _helperAccessor = umbracoHelperAccessor;
            _helperAccessor.TryGetUmbracoHelper(out _helper);
        }

        public List<CustomPropertyInfo> GetAllUmbracoNodeProperties()
        {

            var contentTypes = _contentTypeService.GetAll()?.Where(x => x.AllowedTemplates?.Any() == true);
            var propertyTypes = _contentTypeService.GetAll().Select(x => x.PropertyTypes);
            List<CustomPropertyInfo> result = new List<CustomPropertyInfo>();
            /*
             foreach(var propertyTypeList in propertyTypes)
             {
                 if(propertyTypeList?.Any() == true)
                 {
                     foreach(var propertyType in propertyTypeList?.DistinctBy(x=>x.Alias))
                     {
                         if(propertyType != null && propertyType.PropertyEditorAlias == "Umbraco.DropDown.Flexible" 
                             || propertyType.PropertyEditorAlias == "Umbraco.TrueFalse")
                         {
                             result.Add(propertyType.Name + "-" + propertyType.Alias);
                         }
                     }
                 }
             }
            */

            if (contentTypes?.Any() == true)
            {
                CustomPropertyInfo customPropertyInfoDefault = new CustomPropertyInfo();
                customPropertyInfoDefault.Id = "0";
                customPropertyInfoDefault.Name = "All ContentTypes";

                //CustomPropertyInfo customPropertyInfoDate = new CustomPropertyInfo();
                //customPropertyInfoDate.Id = "createdDate";
                //customPropertyInfoDate.Name = "Created Date";

                result.Add(customPropertyInfoDefault);
                foreach (var contentType in contentTypes)
                {
                    if (contentType != null)
                    {
                        CustomPropertyInfo customPropertyInfo = new CustomPropertyInfo();
                        customPropertyInfo.Id = contentType.Id.ToString();
                        customPropertyInfo.Name = contentType.Name;
                        customPropertyInfo.Alias = contentType.Alias;

                        result.Add(customPropertyInfo);
                    }
                }
            }

            return result;
        }

        public List<CustomPropertyInfo> GetAllProperties(string contentTypeId)
        {
            List<CustomPropertyInfo> customProperties = new List<CustomPropertyInfo>();
            if (!string.IsNullOrEmpty(contentTypeId))
            {
                int contentTypeIdAsInt = Convert.ToInt32(contentTypeId);
                if (contentTypeIdAsInt > 0)
                {
                    var contentType = _contentTypeService.Get(contentTypeIdAsInt);
                    if (contentType != null)
                    {
                        List<IPropertyType> propertyTypes = new List<IPropertyType>();
                        var allPropertiesUnderContent = contentType.PropertyTypes;
                        var allCompPropertiesUnderContent = contentType.CompositionPropertyTypes;

                        if (allPropertiesUnderContent != null)
                        {
                            propertyTypes.AddRange(allPropertiesUnderContent);
                        }
                        if (allCompPropertiesUnderContent != null)
                        {
                            propertyTypes.AddRange(allCompPropertiesUnderContent);
                        }
                        if (propertyTypes?.Any() == true)
                        {
                            foreach (var propertyType in propertyTypes.DistinctBy(x => x.Id))
                            {
                                if (propertyType != null && propertyType.PropertyEditorAlias == "Umbraco.DropDown.Flexible"
                                    || propertyType.PropertyEditorAlias == "Umbraco.TrueFalse" || propertyType.PropertyEditorAlias == "Umbraco.CheckBoxList"
                                    || propertyType.PropertyEditorAlias == "Umbraco.RadioButtonList")
                                {
                                    CustomPropertyInfo customPropertyInfo = new CustomPropertyInfo();
                                    customPropertyInfo.Id = propertyType.DataTypeId.ToString();
                                    customPropertyInfo.Name = propertyType.Name;
                                    customPropertyInfo.Alias = propertyType.Alias;

                                    customProperties.Add(customPropertyInfo);
                                }
                            }
                        }
                    }
                }
                else if (contentTypeIdAsInt == 0)
                {
                    var propertyTypes = _contentTypeService.GetAll().Select(x => x.PropertyTypes);
                    if (propertyTypes?.Any() == true)
                    {
                        foreach (var propertyTypeList in propertyTypes)
                        {
                            if (propertyTypeList?.Any() == true)
                            {
                                foreach (var propertyType in propertyTypeList?.DistinctBy(x => x.Alias))
                                {
                                    if (propertyType != null && propertyType.PropertyEditorAlias == "Umbraco.DropDown.Flexible"
                                        || propertyType.PropertyEditorAlias == "Umbraco.TrueFalse" ||
                                        propertyType.PropertyEditorAlias == "Umbraco.RadioButtonList" || propertyType.PropertyEditorAlias == "Umbraco.CheckBoxList")
                                    {
                                        CustomPropertyInfo customPropertyInfo = new CustomPropertyInfo();
                                        customPropertyInfo.Id = propertyType.DataTypeId.ToString();
                                        customPropertyInfo.Name = propertyType.Name;
                                        customPropertyInfo.Alias = propertyType.Alias;

                                        customProperties.Add(customPropertyInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return customProperties;
        }

        public List<CustomPropertyInfo> GetPropertyValues(string dataTypeId)
        {

            try
            {
                var contentTypea = _dataTypeService.GetByEditorAlias(dataTypeId);
                var contentTypeab = _dataTypeService.GetDataType(dataTypeId);
                if (!string.IsNullOrEmpty(dataTypeId))
                {
                    int dataTypeIdInt = Convert.ToInt32(dataTypeId);

                    var contentType = _dataTypeService.GetDataType(dataTypeIdInt);

                    if (contentType != null)
                    {
                        if (contentType.Configuration != null && !string.IsNullOrWhiteSpace(contentType.EditorAlias))
                        {
                            if (contentType.EditorAlias == "Umbraco.DropDown.Flexible")
                            {
                                var configuration = contentType.Configuration.SafeCast<DropDownFlexibleConfiguration>();
                                if (configuration != null && configuration.Items?.Any() == true)
                                {
                                    List<CustomPropertyInfo> customProperties = new List<CustomPropertyInfo>();
                                    foreach (var item in configuration.Items)
                                    {
                                        CustomPropertyInfo customPropertyInfo = new CustomPropertyInfo();
                                        customPropertyInfo.Id = item.Id.ToString();
                                        customPropertyInfo.Name = item.Value;
                                        customPropertyInfo.Alias = item.Value;

                                        customProperties.Add(customPropertyInfo);
                                    }

                                    return customProperties;
                                }
                            }
                            else if (contentType.EditorAlias == "Umbraco.TrueFalse")
                            {
                                List<CustomPropertyInfo> customProperties = new List<CustomPropertyInfo>();

                                CustomPropertyInfo customPropertyInfo = new CustomPropertyInfo();
                                customPropertyInfo.Id = "0";
                                customPropertyInfo.Name = "true";
                                customPropertyInfo.Alias = "true";

                                customProperties.Add(customPropertyInfo);

                                CustomPropertyInfo customPropertyInfoFalse = new CustomPropertyInfo();
                                customPropertyInfoFalse.Id = "1";
                                customPropertyInfoFalse.Name = "false";
                                customPropertyInfoFalse.Alias = "false";

                                customProperties.Add(customPropertyInfoFalse);

                                return customProperties;
                            }
                            else if (contentType.EditorAlias == "Umbraco.CheckBoxList")
                            {
                                var configuration = contentType.Configuration.SafeCast<ValueListConfiguration>();
                                if (configuration != null && configuration.Items?.Any() == true)
                                {
                                    List<CustomPropertyInfo> customProperties = new List<CustomPropertyInfo>();
                                    foreach (var item in configuration.Items)
                                    {
                                        CustomPropertyInfo customPropertyInfo = new CustomPropertyInfo();
                                        customPropertyInfo.Id = item.Id.ToString();
                                        customPropertyInfo.Name = item.Value;
                                        customPropertyInfo.Alias = item.Value;

                                        customProperties.Add(customPropertyInfo);
                                    }

                                    return customProperties;
                                }
                            }
                            else if (contentType.EditorAlias == "Umbraco.RadioButtonList")
                            {
                                var configuration = contentType.Configuration.SafeCast<ValueListConfiguration>();
                                if (configuration != null && configuration.Items?.Any() == true)
                                {
                                    List<CustomPropertyInfo> customProperties = new List<CustomPropertyInfo>();
                                    foreach (var item in configuration.Items)
                                    {
                                        CustomPropertyInfo customPropertyInfo = new CustomPropertyInfo();
                                        customPropertyInfo.Id = item.Id.ToString();
                                        customPropertyInfo.Name = item.Value;
                                        customPropertyInfo.Alias = item.Value;

                                        customProperties.Add(customPropertyInfo);
                                    }

                                    return customProperties;
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public List<FilteredNodes> FilterNodes(List<ValuesForFilter> filteredDataList)
        {
            if (filteredDataList?.Any() == true)
            {
                IEnumerable<IPublishedContent> rootNodes = _helper.ContentAtRoot();
                List<IContent> allNodes = new List<IContent>();
                List<FilteredNodes> allFilteredNodes = new List<FilteredNodes>();

                if (rootNodes?.Any() == true)
                {
                    foreach (var rootNode in rootNodes)
                    {
                        if (rootNode != null)
                        {
                            foreach (var culture in rootNode.Cultures)
                            {
                                var descesentnodes = rootNode.DescendantsOrSelf(culture.Key);
                                if (descesentnodes?.Any() == true)
                                {
                                    foreach (var node in descesentnodes)
                                    {
                                        if (node != null)
                                        {
                                            var refreshNodeAsContent = _contentService.GetById(node.Id);
                                            if (refreshNodeAsContent != null)
                                            {
                                                var refreshNode = refreshNodeAsContent;
                                                if (refreshNode != null)
                                                {
                                                    allNodes.Add(refreshNode);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }

                if (allNodes?.Any() == true)
                {
                    List<IContent> filteredNodes = allNodes;
                    foreach (var filter in filteredDataList)
                    {
                        if (!string.IsNullOrWhiteSpace(filter.FilteredDate) && !string.IsNullOrWhiteSpace(filter.FilteredEndDate))
                        {
                            DateTime dateTimeStart = Convert.ToDateTime(filter.FilteredDate);
                            DateTime dateTimeEnd = Convert.ToDateTime(filter.FilteredEndDate);
                            if (filter.FilteredMainCategory == "createdDate")
                            {
                                filteredNodes = filteredNodes?.Where(x => x.CreateDate.Date >= dateTimeStart.Date && x.CreateDate.Date <= dateTimeEnd.Date)?.ToList();

                            }
                            else
                            {
                                filteredNodes = filteredNodes?.Where(x => x.UpdateDate.Date >= dateTimeStart.Date && x.UpdateDate.Date <= dateTimeEnd.Date)?.ToList();

                            }

                            if (!string.IsNullOrWhiteSpace(filter.FilteredContentType) && filter.FilteredContentType != "0")
                            {
                                filteredNodes = filteredNodes?.Where(x => x.ContentType.Id == Convert.ToInt32(filter.FilteredContentType))?.ToList();
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(filter.FilteredCategory) && !string.Equals(filter.FilteredCategory, "0"))
                        {
                            filteredNodes = filteredNodes?.Where(x => x.ContentType.Alias == filter.FilteredCategory)?.ToList();

                        }
                        else
                        {
                            filteredNodes = filteredNodes;
                        }
                        if (!string.IsNullOrWhiteSpace(filter.FilteredSubCategory) && !string.IsNullOrWhiteSpace(filter.FilteredValue))
                        {
                            List<IContent> ddlFilteredNodes = new List<IContent>();
                            if (filteredNodes?.Any() == true)
                            {
                                foreach (var filterNode in filteredNodes)
                                {
                                    if (filterNode != null)
                                    {
                                        var type = filterNode.GetValue(filter.FilteredSubCategory)?.GetType()?.Name;
                                        if (!string.IsNullOrWhiteSpace(type))
                                        {
                                            if (string.Equals("int32", type.ToLower()))
                                            {
                                                string value = string.Empty;
                                                if (string.Equals("int32", type.ToLower()))
                                                {
                                                    value = filterNode.GetValue<bool>(filter.FilteredSubCategory).ToString();
                                                }
                                                else
                                                {
                                                    value = filterNode.GetValue<string>(filter.FilteredSubCategory);
                                                }

                                                if (value?.ToLower() == filter.FilteredValue.ToLower())
                                                {
                                                    ddlFilteredNodes.Add(filterNode);
                                                }
                                            }
                                            else if (string.Equals("string", type.ToLower()))
                                            {
                                                var value = filterNode.GetValue<string>(filter.FilteredSubCategory);
                                                List<string> values = CheckAndReturnStringListValues(value);

                                                if (values?.Any() == true && values.Contains(filter.FilteredValue))
                                                {
                                                    ddlFilteredNodes.Add(filterNode);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            filteredNodes = ddlFilteredNodes;

                        }



                    }

                    if (filteredNodes?.Any() == true)
                    {

                        foreach (var nodeAsIContent in filteredNodes.DistinctBy(x => x.Id))
                        {
                            if (nodeAsIContent != null)
                            {
                                //var node = _helper.Content(nodeAsIContent.Id);
                                FilteredNodes filteredNode = new FilteredNodes
                                {
                                    NodeId = nodeAsIContent.Id,
                                    NodeName = nodeAsIContent.Name,
                                    NodeType = nodeAsIContent.ContentType.Alias,
                                    NodeUrl = string.Empty,
                                    NodeFilterCondition = string.Empty,
                                    NodeUmbracoUrl = $"/umbraco#/content/content/edit/{nodeAsIContent.Id}",
                                    NodeCreatedDate = nodeAsIContent.CreateDate.ToString("dd-MM-yyyy"),
                                    NodeUpdatedDate = nodeAsIContent.UpdateDate.ToString("dd-MM-yyyy")
                                };

                                allFilteredNodes.Add(filteredNode);
                            }
                        }

                    }
                }

                return allFilteredNodes;
            }
            return null;
        }

        private List<string> CheckAndReturnStringListValues(string strInput)
        {
            List<string> valuesAsString = new List<string>();
            if (!string.IsNullOrWhiteSpace(strInput))
            {
                strInput = strInput.Trim();
                if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || // For object
                    (strInput.StartsWith("[") && strInput.EndsWith("]")))   // For array
                {
                    try
                    {
                        valuesAsString = JsonConvert.DeserializeObject<List<string>>(strInput);
                        return valuesAsString;
                    }
                    catch (JsonReaderException)
                    {

                    }
                    catch (Exception) // some other exception
                    {

                    }
                }
                else
                {
                    valuesAsString.Add(strInput);

                    return valuesAsString;
                }
            }

            return valuesAsString;


        }
    }
}
