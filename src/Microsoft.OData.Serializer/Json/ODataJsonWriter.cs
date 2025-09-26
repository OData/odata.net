using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json;

public abstract class ODataJsonWriter<T, TCustomState> : ODataWriter<T, ODataWriterState<TCustomState>>
{
}
