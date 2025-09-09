using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public abstract class ODataAsyncPropertyWriter<TDeclaringType> : ODataAsyncPropertyWriter<TDeclaringType, DefaultState>
{
}