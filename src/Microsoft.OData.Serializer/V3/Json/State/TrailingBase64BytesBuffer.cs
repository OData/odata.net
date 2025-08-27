using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.State;

[InlineArray(2)]
internal struct TrailingBase64BytesBuffer
{
    byte _byte;
}
