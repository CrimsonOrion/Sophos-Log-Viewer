using MahApps.Metro.Controls.Dialogs;

using Prism.Mvvm;
using Prism.Regions;

using SophosLogViewer.Core.Models;
using SophosLogViewer.Core.Processors;

using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using SophosLogViewer.Core;
using System.Linq;

namespace LogViewer.Module.ViewModels;
public class LogViewerViewModel : BindableBase, INavigationAware
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IFileProcessor _fileProcessor;
    private readonly IErrorHandler _errorHandler;

    #region LogViewer View Properties

    public static string Title => "Log Viewer";

    private string _logFileName;
    public string LogFileName
    {
        get { return _logFileName; }
        set { SetProperty(ref _logFileName, value); CheckLogFileAsync(); }
    }

    private ObservableCollection<FirewallLogModel> _firewallLogEntries;
    public ObservableCollection<FirewallLogModel> FirewallLogEntries
    {
        get { return _firewallLogEntries; }
        set { SetProperty(ref _firewallLogEntries, value); }
    }

    #endregion

    #region Delegate Commands

    #endregion

    #region Constructor

    public LogViewerViewModel(IDialogCoordinator dialogCoordinator, IFileProcessor fileProcessor, IErrorHandler errorHandler)
    {
        _dialogCoordinator = dialogCoordinator;
        _fileProcessor = fileProcessor;
        _errorHandler = errorHandler;
    }

    #endregion

    #region Methods

    #region Navigation

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
    public void OnNavigatedTo(NavigationContext navigationContext) { }

    #endregion

    #region Private

    async void CheckLogFileAsync()
    {
        if (string.IsNullOrEmpty(LogFileName) || 
            await _dialogCoordinator.ShowMessageAsync(this, "Confirm", "Are you sure you want to process this file?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = "Processing Firewall Log File...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
        controller.SetIndeterminate();
        controller.SetMessage(message);
        await Task.Run(() => Thread.Sleep(1));
        try
        {
            using CancellationTokenSource _tokensource = new();
            controller.Canceled += (object? sender, EventArgs e) =>
            {
                message = "Cancelling";
                controller.SetMessage(message);
                _tokensource.Cancel();
            };

            // Process File
            controller.SetMessage(message);
            await Task.Run(() => Thread.Sleep(1));
            var list = await Task.Run(() => _fileProcessor.ProcessFirewallLogData(new() { LogFileName }));

            List<FirewallLogModel> list2 = new();
            for (int i = 0; i < 1000; i++)
            {
                list2.Add(list[i]);
            }

            FirewallLogEntries = new(list2);


            title = "Completed";
            message = "";
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem processing the firewall log file.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }

    #endregion

    #endregion
}