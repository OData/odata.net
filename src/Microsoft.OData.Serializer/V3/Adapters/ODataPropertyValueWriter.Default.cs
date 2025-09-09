using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

// TODO: internal because this is not yet supported by the ODataTypeInfoFactory.
internal abstract class ODataPropertyValueWriter<TDeclaringType> : ODataPropertyValueWriter<TDeclaringType, DefaultState>
{
}
