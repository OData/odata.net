using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Models;

public class AccessControlList
{
    public int Version { get; set; } // not mapped to OData
    public IList<AccessControlEntry> AccessControlEntries { get; set; }
}
