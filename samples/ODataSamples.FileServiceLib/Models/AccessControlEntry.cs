using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Schema;

namespace ODataSamples.FileServiceLib.Models;

[ODataType($"{SchemaConstants.Namespace}.AccessControlEntry")]
public class AccessControlEntry
{
    public AccessRight AccessRight { get; set; }

    public Claim Claim { get; set; }
}