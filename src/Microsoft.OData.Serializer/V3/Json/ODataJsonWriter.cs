using Microsoft.OData.Serializer.V3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

public abstract class ODataJsonWriter<T, TCustomState> : ODataWriter<T, ODataJsonWriterState<TCustomState>>
{
}
