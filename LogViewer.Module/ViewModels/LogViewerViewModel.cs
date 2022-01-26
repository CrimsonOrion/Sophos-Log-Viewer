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
using Prism.Commands;

namespace LogViewer.Module.ViewModels;
public class LogViewerViewModel : BindableBase, INavigationAware
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IFileProcessor _fileProcessor;
    private readonly IErrorHandler _errorHandler;
    List<FirewallLogModel> _list = new();

    #region LogViewer View Properties

    public static string Title => "Log Viewer";

    private string _logFileName;
    public string LogFileName
    {
        get { return _logFileName; }
        set { SetProperty(ref _logFileName, value); CheckLogFileAsync(); }
    }

    private ObservableCollection<FirewallLogModel> _firewallLogEntries = new();
    public ObservableCollection<FirewallLogModel> FirewallLogEntries
    {
        get { return _firewallLogEntries; }
        set { SetProperty(ref _firewallLogEntries, value); }
    }

    private ObservableCollection<string> _srcIPList;
    public ObservableCollection<string> SrcIPList
    {
        get { return _srcIPList; }
        set { SetProperty(ref _srcIPList, value); }
    }

    private ObservableCollection<string> _dstIPList;
    public ObservableCollection<string> DstIPList
    {
        get { return _dstIPList; }
        set { SetProperty(ref _dstIPList, value); }
    }

    private ObservableCollection<string> _dstPortList;
    public ObservableCollection<string> DstPortList
    {
        get { return _dstPortList; }
        set { SetProperty(ref _dstPortList, value); }
    }

    private ObservableCollection<string> _fwRuleList;
    public ObservableCollection<string> FwRuleList
    {
        get { return _fwRuleList; }
        set { SetProperty(ref _fwRuleList, value); }
    }

    private string _selectedSrcIP = "";
    public string SelectedSrcIP
    {
        get { return _selectedSrcIP; }
        set { SetProperty(ref _selectedSrcIP, value); }
    }

    private string _selectetDstIP = "";
    public string SelectedDstIP
    {
        get { return _selectetDstIP; }
        set { SetProperty(ref _selectetDstIP, value); }
    }

    private string _selectedDstPort = "";
    public string SelectedDstPort
    {
        get { return _selectedDstPort; }
        set { SetProperty(ref _selectedDstPort, value); }
    }

    private string _selectedFwRule = "";
    public string SelectedFwRule
    {
        get { return _selectedFwRule; }
        set { SetProperty(ref _selectedFwRule, value); }
    }

    #endregion

    #region Delegate Commands

    public DelegateCommand ApplyFilterCommand => new(DisplayList);

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
            _list = await Task.Run(() => _fileProcessor.ProcessFirewallLogData(new() { LogFileName }));

            ResetLists();

            title = "";
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
            if (!string.IsNullOrEmpty(title))
            {
                await _dialogCoordinator.ShowMessageAsync(this, title, message);
            }
        }
    }

    void ResetLists()
    {
        SrcIPList = new(_list.OrderBy(_ => _.SrcIP).GroupBy(_ => _.SrcIP).Select(_ => _.Key).Prepend(""));
        DstIPList = new(_list.OrderBy(_ => _.DstIP).GroupBy(_ => _.DstIP).Select(_ => _.Key).Prepend(""));
        DstPortList = new(_list.OrderBy(_ => _.DstPort).GroupBy(_ => _.DstPort).Select(_ => _.Key).Prepend(""));
        FwRuleList = new(_list.OrderBy(_ => _.FWRule).GroupBy(_ => _.FWRule).Select(_ => _.Key).Prepend(""));

        FirewallLogEntries = new();
    }

    async void DisplayList()
    {
        var title = "Please wait...";
        var message = "Processing Firewall Log File...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
        controller.SetIndeterminate();
        controller.SetMessage(message);
        await Task.Run(() => Thread.Sleep(1));

        if (string.IsNullOrEmpty(SelectedSrcIP) && string.IsNullOrEmpty(SelectedDstIP) && string.IsNullOrEmpty(SelectedDstPort) && string.IsNullOrEmpty(SelectedFwRule))
        {
            ResetLists();
            await controller.CloseAsync();
            return;
        }

        List<FirewallLogModel> filteredList = new(_list);
        FirewallLogEntries = new();

        if (!string.IsNullOrEmpty(SelectedSrcIP))
        {
            filteredList = filteredList.Where(_ => _.SrcIP == SelectedSrcIP).ToList();
        }

        if (!string.IsNullOrEmpty(SelectedDstIP))
        {
            filteredList = filteredList.Where(_ => _.DstIP == SelectedDstIP).ToList();
        }

        if (!string.IsNullOrEmpty(SelectedDstPort))
        {
            filteredList = filteredList.Where(_ => _.DstPort == SelectedDstPort).ToList();
        }

        if (!string.IsNullOrEmpty(SelectedFwRule))
        {
            filteredList = filteredList.Where(_ => _.FWRule == SelectedFwRule).ToList();
        }

        SrcIPList = SelectedSrcIP == String.Empty ? new(filteredList.OrderBy(_ => _.SrcIP).GroupBy(_ => _.SrcIP).Select(_ => _.Key).Prepend("")) : SrcIPList;
        DstIPList = SelectedDstIP == String.Empty ? new(filteredList.OrderBy(_ => _.DstIP).GroupBy(_ => _.DstIP).Select(_ => _.Key).Prepend("")) : DstIPList;
        DstPortList = SelectedDstPort == String.Empty ? new(filteredList.OrderBy(_ => _.DstPort).GroupBy(_ => _.DstPort).Select(_ => _.Key).Prepend("")) : DstPortList;
        FwRuleList = SelectedFwRule == String.Empty ? new(filteredList.OrderBy(_ => _.FWRule).GroupBy(_ => _.FWRule).Select(_ => _.Key).Prepend("")) : FwRuleList;

        var count = filteredList.Count < 2000 ? filteredList.Count : 2000;

        for (int i = 0; i < count; i++)
        {
            FirewallLogEntries.Add(filteredList[i]);
        }

        await controller.CloseAsync();
    }

    #endregion

    #endregion
}