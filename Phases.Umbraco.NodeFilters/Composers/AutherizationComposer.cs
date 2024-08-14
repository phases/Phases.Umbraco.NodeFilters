using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
using Phases.Umbraco.NodeFilters.Controllers.FilterNodes;

namespace Phases.Umbraco.NodeFilters.Core.Composers
{
    public class AutherizationComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter(nameof(FilterNodesApiController))
                {
                    Endpoints = app => app.UseEndpoints(endpoints =>
                    {
                        var globalSettings = app.ApplicationServices
                            .GetRequiredService<IOptions<GlobalSettings>>().Value;
                        var hostingEnvironment = app.ApplicationServices
                            .GetRequiredService<IHostingEnvironment>();
                        var backofficeArea = Constants.Web.Mvc.BackOfficePathSegment;

                        var rootSegment = $"{globalSettings.GetUmbracoMvcArea(hostingEnvironment)}/{backofficeArea}";
                        var areaName = "filternodes";
                        endpoints.MapUmbracoRoute<FilterNodesApiController>(rootSegment, areaName, areaName);
                    })
                });
            });
        }
    }
}
