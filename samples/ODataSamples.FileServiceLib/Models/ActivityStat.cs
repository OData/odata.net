using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Schema;

namespace ODataSamples.FileServiceLib.Models;

[ODataType($"{SchemaConstants.Namespace}.ActivityStat")]
public class ActivityStat
{
    public string Id { get; set; }
    public FileAccessType Activity { get; set; }
    public string Actor { get; set; }
    public DateTimeOffset ActivityDateTime { get; set; }
}
