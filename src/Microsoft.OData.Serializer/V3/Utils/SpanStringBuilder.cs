using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Utils;

internal ref struct SpanStringBuilder(Span<char> buffer)
{
    Span<char> _buffer = buffer;
    private int _pos = 0;


    public void Append(char value)
    {
        Debug.Assert(_buffer.Length > _pos + 1);
        _buffer[_pos++] = value;
    }

    public void Append(ReadOnlySpan<char> value)
    {
        Debug.Assert(_buffer.Length >= _pos + value.Length);
        value.CopyTo(_buffer[_pos..]);
        _pos += value.Length;
    }

    public readonly ReadOnlySpan<char> WrittenSpan => _buffer[.._pos];
}
