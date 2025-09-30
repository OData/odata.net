using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Streaming;

public interface IStreamingSource
{
    IAsyncEnumerable<ReadOnlyMemory<byte>> GetStreamingBytes();
}
