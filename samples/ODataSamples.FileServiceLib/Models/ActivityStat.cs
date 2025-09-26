namespace ODataSamples.FileServiceLib.Models;

public class ActivityStat
{
    public string Id { get; set; }
    public FileAccessType Activity { get; set; }
    public string Actor { get; set; }
    public DateTimeOffset ActivityDateTime { get; set; }
}
