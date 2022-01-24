using Microsoft.Win32;

using System.Windows;
using System.Windows.Controls;

namespace LogViewer.Module.Views;
/// <summary>
/// Interaction logic for LogViewerView.xaml
/// </summary>
public partial class LogViewerView : UserControl
{
    public LogViewerView() => InitializeComponent();

    private void UploadButton_Click(object sender, RoutedEventArgs e)
    {
        LogFileName.Text = string.Empty;
        OpenFileDialog dialog = new()
        {
            Title = "Select Firewall Log File",
            Filter = "All files (*.*)|*.*",
            FilterIndex = 0,
            Multiselect = true,
            RestoreDirectory = true
        };

        if (dialog.ShowDialog() == true)
        {
            LogFileName.Text = dialog.FileName;
        }
    }
}
