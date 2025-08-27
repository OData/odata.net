using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.State;

[InlineArray(TrailingBase64BytesBuffer.Length)]
internal struct TrailingBase64BytesBuffer
{
    public const int Length = 3;
    byte _byte;
}
