namespace SophosLogViewer.Core;
public class ErrorHandler : IErrorHandler
{
    public ErrorHandler() { }

    public async Task<string> ReportErrorAsync(Exception ex) => await Task.Run(() => $"Error: {ex.Message}");
}