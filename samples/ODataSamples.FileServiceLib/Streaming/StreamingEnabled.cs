using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Streaming;

/// <summary>
/// Based class for resource which contain text or binary
/// properties that can be streamed.
/// </summary>
public class StreamingEnabled
{
    /// <summary>
    /// If this object contains streamable data,
    /// then each entry in the dictionary is a mapping between a property
    /// and its streaming source.
    /// </summary>
    public IDictionary<string, IStreamingSource>? StreamableProperties { get; set; }
}
