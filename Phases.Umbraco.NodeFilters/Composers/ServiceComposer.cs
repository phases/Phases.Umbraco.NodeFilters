using Microsoft.Extensions.DependencyInjection;
using Phases.Umbraco.NodeFilters.Services.FilterNodes;
using Phases.Umbraco.NodeFilters.Services.Interfaces.FilterNodes;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Phases.Umbraco.NodeFilters.Composers
{
    public class ServiceComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IFilterNodeServices, FilterNodeServices>();
        }
    }
}
