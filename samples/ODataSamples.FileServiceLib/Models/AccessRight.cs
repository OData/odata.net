using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Models;

[ODataType($"{SchemaConstants.Namespace}.AccessControlRight")]
public enum AccessRight
{
    Read = 1,
    Write = 2
}
