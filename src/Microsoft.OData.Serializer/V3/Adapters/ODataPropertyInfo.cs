using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataPropertyInfo
{
    private string name;

    public string? Name
    {
        get => name;
        set
        {
            ArgumentNullException.ThrowIfNullOrEmpty(value);
            if (!Utf8Name.IsEmpty)
            {
                throw new Exception("Cannot modify property info name");
            }

            Utf8Name = Encoding.UTF8.GetBytes(value);
            name = value;
        }
    }

    public Memory<byte> Utf8Name { get; private set; }

    // TODO: Code smell that this is defined here and GetStreamValue defined in the child class?
    // Also what if the choice to leave the stream
    // open depends on the state or the actual stream value?
    /// <summary>
    /// If set to true, the stream returned by GetStreamingValue in the child class will not be disposed. By default,
    /// the stream will be disposed after the writer finishes reading from it.
    /// </summary>
    public bool? LeaveStreamOpen { get; init; }
}
