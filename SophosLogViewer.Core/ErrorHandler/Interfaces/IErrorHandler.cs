namespace SophosLogViewer.Core;

public interface IErrorHandler
{
    Task<string> ReportErrorAsync(Exception ex);
}