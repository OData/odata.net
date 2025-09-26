using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Models;

[ODataType($"{SchemaConstants.Namespace}.FileContent")]
public class FileContent
{
    public string? Text { get; set; }
    public string? Annotation { get; set; }

    [ODataIgnore] // This attribute is unnecessary since OData will automatically ignore any property not defined in the EDM model
    public string? Ext { get; set; } // not defined in Edm model
}
