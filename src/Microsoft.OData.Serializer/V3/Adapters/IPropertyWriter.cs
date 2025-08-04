using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface IPropertyWriter<TDeclaringType, TCustomState>
{
    // TODO: consider overloads that accept ReadOnlySpan<char> and ReadOnlySpan<byte>
    ValueTask WriteProperty<T>(TDeclaringType resource, string name, T Value, ODataJsonWriterState<TCustomState> state);
    ValueTask WriteProperty<T>(TDeclaringType resource, ODataPropertyInfo<TDeclaringType, TCustomState> propertyInfo, T value, ODataJsonWriterState<TCustomState> state);
    ValueTask WriteProperty(
        TDeclaringType resource,
        ODataPropertyInfo<TDeclaringType, TCustomState> propertyInfo,
        ODataJsonWriterState<TCustomState> state);
}
