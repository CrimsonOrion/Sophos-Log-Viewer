using ControlzEx.Theming;

using LogViewer.Module;

using MahApps.Metro.Controls.Dialogs;

using Microsoft.Extensions.Configuration;

using Prism.Ioc;
using Prism.Modularity;

using SophosLogViewer.Core;
using SophosLogViewer.Core.Processors;

using System.Windows;

namespace SophosLogViewer;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
        ThemeManager.Current.SyncTheme();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Set up the configuration using the appSettings.json file.
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", false, true)
            .Build();

        //GlobalConfig.DepositInfoConfiguration = new()
        //{
        //    MinimumDepositElectric = !string.IsNullOrEmpty(configuration["Deposit Info:Minimum Deposit-Electric"]) && decimal.TryParse(configuration["Deposit Info:Minimum Deposit-Electric"], out var mde) ? mde : 0m,
        //    MinimumDepositWater = !string.IsNullOrEmpty(configuration["Deposit Info:Minimum Deposit-Water"]) && decimal.TryParse(configuration["Deposit Info:Minimum Deposit-Water"], out var mdw) ? mdw : 0m,
        //    MinimumBillElectric = !string.IsNullOrEmpty(configuration["Deposit Info:Minimum Bill-Electric"]) && decimal.TryParse(configuration["Deposit Info:Minimum Bill-Electric"], out var mbe) ? mbe : 0m,
        //    MinimumBillWater = !string.IsNullOrEmpty(configuration["Deposit Info:Minimum Bill-Water"]) && decimal.TryParse(configuration["Deposit Info:Minimum Bill-Water"], out var mbw) ? mbw : 0m,
        //    MinimumAdditionalDeposit = !string.IsNullOrEmpty(configuration["Deposit Info:Minimum Additional Deposit"]) && decimal.TryParse(configuration["Deposit Info:Minimum Additional Deposit"], out var mad) ? mad : 0m,
        //};

        containerRegistry
            .RegisterInstance<IDialogCoordinator>(new DialogCoordinator())

            .Register<IErrorHandler, ErrorHandler>()
            .Register<IFileProcessor, FileProcessor>()
            ;
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) => moduleCatalog.AddModule<LogViewerModule>();
}