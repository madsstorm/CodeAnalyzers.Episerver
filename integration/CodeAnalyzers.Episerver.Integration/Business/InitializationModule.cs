using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace CodeAnalyzers.Episerver.Integration.Business
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class InitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var contentEvents = context.Locate.Advanced.GetInstance<IContentEvents>();

            contentEvents.SavingContent += ContentEvents_SavingContent;
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        private void ContentEvents_SavingContent(object sender, EPiServer.ContentEventArgs e)
        {
        }
    }
}