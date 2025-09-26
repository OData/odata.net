using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Serialization.OData;

public readonly struct ODataCustomState
{
    public IdPropertySerializer? IdSerializer { get; init; }
}
