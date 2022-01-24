using LogViewer.Module.Views;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using SophosLogViewer.Core;

namespace LogViewer.Module;
public class LogViewerModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) => containerProvider
            .Resolve<IRegionManager>()
            .RegisterViewWithRegion(KnownRegionNames.MainRegion, typeof(LogViewerView));
    public void RegisterTypes(IContainerRegistry containerRegistry) => containerRegistry.RegisterForNavigation<LogViewerView>();
}