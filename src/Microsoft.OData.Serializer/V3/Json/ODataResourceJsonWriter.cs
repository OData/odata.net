using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

internal class ODataResourceJsonWriter<T>(ODataResourceTypeInfo<T> typeInfo) : ODataWriter<T, ODataJsonWriterState>
{
    public override async ValueTask Write(T value, ODataJsonWriterState state)
    {
        state.Stack.Push();
        state.Stack.Current.ResourceTypeInfo = typeInfo;
        var jsonWriter = state.JsonWriter;
        jsonWriter.WriteStartObject();
        // This makes the following assumptions:
        // - all properties defined in the resourceInfo should be written
        // - the properties are written in the order they are defined in the resource info
        // - each property to be written is available (e.g. could be null or missing)
        for (int i = 0; i < typeInfo.Properties.Count; i++)
        {
            var propertyInfo = typeInfo.Properties[i];

            await this.WriteProperty(value, propertyInfo, state);

            // when we implement re-entrancy, we should
            // return if property was not written completely.
            //if (!propertyWritten)
            //{
            //    // assume the property has stored enough state to resume writing later
            //    return false;
            //}

            if (state.ShouldFlush())
            {
                // return false; // TODO: store state and return false if re-entrancy implemented
                // TODO: flush or return if re-entrancy implemented
            }

            // if async source needs more data, we should return false as well
        }

        jsonWriter.WriteEndObject();
        state.Stack.Pop();
    }

    private async ValueTask WriteProperty(T resource, ODataPropertyInfo<T> propertyInfo, ODataJsonWriterState state)
    {
        var jsonWriter = state.JsonWriter;
        // add this property to the state, including current index
        // TODO: check annotations.

        // what if the property doesn't exist on the value (e.g. dictionary where only some properties are present)
        // do we expect to write null those properties as null or skip them?

        // How do we handle property selection?
        // examples:
        // - resource exposes a SelectProperties() that returns IEnumerable<ODataPropertyInfo<TDeclaringType>>
        // - resource or property exposes a ShouldWrite() or ShouldSkipProperty() that is evaluated per property?
        //   - this might rely on the implementation accessing the property value twice (e.g. to check whether null or missing)
        //   - should we assume that property access is O(1)? If not, might not be a good idea to force multiple lookups of the same value
        jsonWriter.WritePropertyName(propertyInfo.Name);

        // Another option is for propertyInfo to expose a GetXX method, e.g.
        // if propertyInfo.IsString(), string value = propertyInfo.GetString(value);
        // then we take care of writing it.
        // This would work well if property if O(1), but what if O(n)?
        // e.g. what if value is an IEnumerable<(Property, Value)> (e.g. ODataResource.Properties)
        // This will make this loop quadratic.
        // What if the value handed us its properties via common representation
        // resourceInfo.GetProperties(value) that returns IEnumerable<CustomPropertyRepresentation>
        // problem is if we make the property repersentation generic, we might have to make the entire
        // stack generic on that property representation.
        // be we could take the property representation and call an adapter to retrieve a value.
        // But what if it's a POCO class, what's the native representation? A string?
        // so it would check the string property name in an if statement chain like in v2??
        // If we expose WriteValue, are we sure the user will preserve the state correctly
        // on resumable writes?
        //bool propertyWritten = propertyInfo.WriteValue(value, state);
        await propertyInfo.WriteValue(resource, state);

        // when we implement re-entrancy, we should
        // return if property was not written completely.
        //if (!propertyWritten)
        //{
        //    // assume the property has stored enough state to resume writing later
        //    return false;
        //}
    }
}