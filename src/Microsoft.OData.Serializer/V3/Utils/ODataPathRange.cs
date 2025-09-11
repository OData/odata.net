using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Utils;

/// <summary>
/// A slice of an ODataPath
/// starting from the beginning of the path up
/// to the specified number of segments.
/// </summary>
internal struct ODataPathRange(ODataPath path, int count)
{
    public int Count { get; } = count;
    public ODataPathSegment this[int index]
    {
        get
        {
            Debug.Assert(index < Count);
            return path[index];
        }
    }
}
