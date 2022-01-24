using SophosLogViewer.Core.Models;

namespace SophosLogViewer.Core.Processors;

public interface IFileProcessor
{
    List<FirewallLogModel> ProcessFirewallLogData(List<string> logfilePaths);
}