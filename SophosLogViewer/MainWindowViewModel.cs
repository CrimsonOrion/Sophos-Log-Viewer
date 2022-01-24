using Prism.Mvvm;
using Prism.Regions;

using SophosLogViewer.Core;

namespace SophosLogViewer;
public class MainWindowViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;

    #region Main Window Properties

    public static string Title => $"Sophos Log Viewer";

    #endregion

    #region Constructor

    public MainWindowViewModel(IRegionManager regionManager) => _regionManager = regionManager;

    #endregion

    #region Private Methods

    private void Navigate(string navigationPath, NavigationParameters navigationParameters = null) => _regionManager.RequestNavigate(KnownRegionNames.MainRegion, navigationPath, navigationParameters);

    #endregion
}