using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal partial class StreamValueWriter<TCustomState> : IStreamValueWriter<TCustomState>
{
	public void WriteStringSegment(ReadOnlySpan<char> value, bool isFinalBlock, ODataWriterState<TCustomState> state)
	{
		throw new NotImplementedException();
	}

	public void WriteStringSegment(ReadOnlySpan<byte> value, bool isFinalBlock, ODataWriterState<TCustomState> state)
	{
		throw new NotImplementedException();
	}
}
