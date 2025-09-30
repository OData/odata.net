using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Schema;

namespace ODataSamples.FileServiceLib.Models;

[ODataType($"{SchemaConstants.Namespace}.FileAccessType")]
public enum FileAccessType
{
    Unknown = 0,
    Access = 1,
    Edit = 2 // not mapped to OData
}