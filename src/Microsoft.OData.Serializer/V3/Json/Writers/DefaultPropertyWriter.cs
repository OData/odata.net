using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class DefaultPropertyWriter<TCustomState> : IPropertyWriter<TCustomState>
{
    public static readonly DefaultPropertyWriter<TCustomState> Instance = new();

    public bool WriteProperty<T>(ReadOnlySpan<char> propertyName, T value, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        if (state.Stack.Current.PropertyProgress < PropertyProgress.Name)
        {
            jsonWriter.WritePropertyName(propertyName);
            state.Stack.Current.PropertyProgress = PropertyProgress.Name;
        }

        return state.WriteValue(value);
    }
}
