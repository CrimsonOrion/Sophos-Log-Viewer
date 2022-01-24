namespace SophosLogViewer.Core.Models;
public class FirewallLogModel
{
    public string TimeStamp { get; set; }
    public string ID { get; set; }
    public string Severity { get; set; }
    public string Sys { get; set; }
    public string Sub { get; set; }
    public string Name { get; set; }
    public string Action { get; set; }
    public string FWRule { get; set; }
    public string Initf { get; set; }
    public string Outitf { get; set; }
    public string Mark { get; set; }
    public string App { get; set; }
    public string SrcMAC { get; set; }
    public string DstMAC { get; set; }
    public string SrcIP { get; set; }
    public string DstIP { get; set; }
    public string Proto { get; set; }
    public string Length { get; set; }
    public string TOS { get; set; }
    public string Prec { get; set; }
    public string TTL { get; set; }
    public string Type { get; set; }
    public string Code { get; set; }
    public string SrcPort { get; set; }
    public string DstPort { get; set; }
    public string TCPFlags { get; set; }
}