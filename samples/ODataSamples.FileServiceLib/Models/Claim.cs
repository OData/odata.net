using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Models;

[ODataType($"{SchemaConstants.Namespace}.Claim")]
public class Claim
{
    public string? Type { get; set; }
    public string? Value { get; set; }
}
