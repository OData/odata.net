using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Schema;
using ODataSamples.FileServiceLib.Serialization.OData;
using ODataSamples.FileServiceLib.Streaming;

namespace ODataSamples.FileServiceLib.Models;

[ODataType($"{SchemaConstants.Namespace}.FileContent")]
public class FileContent : StreamingEnabled
{
    [ODataValueWriter(typeof(StreamingTextPropertyWriter<FileContent, string>))]
    public string? Text { get; set; }

    [ODataValueWriter(typeof(StreamingTextPropertyWriter<FileContent, string>))]
    public string? Annotation { get; set; }

    [ODataIgnore] // This attribute is unnecessary since OData will automatically ignore any property not defined in the EDM model
    public string? Ext { get; set; } // not defined in Edm model
}
