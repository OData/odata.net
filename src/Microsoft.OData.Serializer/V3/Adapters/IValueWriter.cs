using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface IValueWriter
{
    // we don't pass state because this is passed to methods that already accept a state parameter
    ValueTask WriteValue<T>(T value);
}
