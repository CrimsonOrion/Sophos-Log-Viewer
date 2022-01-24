using SophosLogViewer.Core.Models;

using System.IO.Compression;

namespace SophosLogViewer.Core.Processors;
public class FileProcessor : IFileProcessor
{
    private readonly IErrorHandler _errorHandler;

    public FileProcessor(IErrorHandler errorHandler) => _errorHandler = errorHandler;

    public List<FirewallLogModel> ProcessFirewallLogData(List<string> logfilePaths)
    {
        List<FirewallLogModel> fullList = new();

        List<string> entries = new() { "Time,Action,Rule,SourceIP,SrcPort,DestIP,DstPort" };

        foreach (string logfile in logfilePaths)
        {
            fullList.AddRange(ProcessFile(logfile));
        }
        File.WriteAllLines(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Rules.csv"), entries);

        return fullList;
    }

    static List<FirewallLogModel> ProcessFile(string logfile)
    {
        List<string> entries = new();
        List<FirewallLogModel> fullList = new();
        FileInfo fileInfo = new(logfile);
        List<string> lines = ReadZippedLogfile(fileInfo);

        foreach (string line in lines)
        {
            var entry = line.Split("\" ");

            FirewallLogModel model = new();

            foreach (var property in entry)
            {
                if (string.IsNullOrEmpty(property)) continue;

                if (property.Contains(": id="))
                {
                    var timeStamp = property.Split(' ')[0];
                    var id = property.Split("id=\"")[1];
                    model.TimeStamp = timeStamp;
                    model.ID = id;
                }
                else
                {
                    var valuePair = property.Split("=\"");
                    var id = valuePair[0].Trim();
                    var value = valuePair[1].Trim();

                    switch (id)
                    {
                        case "severity":
                            model.Severity = value;
                            break;
                        case "sys":
                            model.Sys = value;
                            break;
                        case "sub":
                            model.Sub = value;
                            break;
                        case "name":
                            model.Name = value;
                            break;
                        case "action":
                            model.Action = value;
                            break;
                        case "fwrule":
                            model.FWRule = value;
                            break;
                        case "initf":
                            model.Initf = value;
                            break;
                        case "outitf":
                            model.Outitf = value;
                            break;
                        case "mark":
                            model.Mark = value;
                            break;
                        case "app":
                            model.App = value;
                            break;
                        case "srcmac":
                            model.SrcMAC = value;
                            break;
                        case "dstmac":
                            model.DstMAC = value;
                            break;
                        case "srcip":
                            model.SrcIP = value;
                            break;
                        case "dstip":
                            model.DstIP = value;
                            break;
                        case "proto":
                            model.Proto = value;
                            break;
                        case "length":
                            model.Length = value;
                            break;
                        case "tos":
                            model.TOS = value;
                            break;
                        case "prec":
                            model.Prec = value;
                            break;
                        case "ttl":
                            model.TTL = value;
                            break;
                        case "type":
                            model.Type = value;
                            break;
                        case "code":
                            model.Code = value;
                            break;
                        case "srcport":
                            model.SrcPort = value;
                            break;
                        case "dstport":
                            model.DstPort = value;
                            break;
                        case "tcpflags":
                            model.TCPFlags = value;
                            break;
                        default:
                            break;
                    }
                }
            }

            //var rules = new[] { "206", "207", "209" };

            //var conditions = (rules.Any(model.FWRule.Contains)) && (model.DstIP == "10.20.3.10" || model.SrcIP == "10.20.3.10");
            //var conditions = (model.DstIP == "10.168.34.3" || model.SrcIP == "10.200.15.5") && model.Action.ToLower() == "drop";
            //var conditions = model.DstIP == "10.254.15.251" && (model.SrcIP == "10.200.15.5" || model.SrcIP == "10.200.15.53" || model.SrcIP == "10.200.15.52" || model.SrcIP == "10.200.16.15");
            //var conditions = model.SrcIP == "10.200.15.5" && model.SrcPort == "53";
            var conditions = true;

            if (conditions)
            {
                fullList.Add(model);
                entries.Add($"{model.TimeStamp},{model.Action},{model.FWRule},{model.SrcIP},{model.SrcPort},{model.DstIP},{model.DstPort}");
            }
        }

        return fullList;
    }

    private static List<string> ReadZippedLogfile(FileInfo file)
    {
        List<string> list = new();
        using FileStream fileStream = File.OpenRead(file.FullName);
        using GZipStream gzStream = new(fileStream, CompressionMode.Decompress);
        using StreamReader reader = new(gzStream);
        while (!reader.EndOfStream)
        {
            list.Add(reader.ReadLine());
        }

        return list;
    }
}