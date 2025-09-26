using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Models;

// TODO: we should add attribute to allow skipping null properties during serialization
[ODataType($"{SchemaConstants.Namespace}.AccessControlList")]
public class AccessControlList
{
    public int Version { get; set; } // not mapped to OData
    public IList<AccessControlEntry> AccessControlEntries { get; set; }
}
