using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataPropertyInfo<TDeclaringType> : ODataBasePropertyInfo
{
    public override string Name { get; set; }

    // TODO if we write JSON, we need to specify JsonState so that it can access the JsonWriter
    // but what if we want to write primitive raw values (e.g. $value)? then we need to specify two separate
    // WriteValue hooks? e.g. WriteJsonValue and WriteRawValue? That could be tedious
    // We could expose a GetValue but that would have to return object and require boxing
    // we could expose different GetValue properties, e.g. GetString(), GetInt32(), etc. and the user would only implement
    // the one that's appropriate for their type.
    // We could also expose a single WriteValue that accepts an IODataValueWriter interface that exposes overloads for different types,
    // then the user just calls the one the overload. This would not accept a state. This assumes the value extraction
    // is independent of state. But that only works for primitive values with short values. But what about strings that are too long?
    // For strings, we might need to write in chunks, so we need resumability, which requires state, but which state do we pass here?
    // So the choice is either to have two separate WriteValue methods for JSON and raw values for primitive types, each
    // which accepts a writer-specific state
    // Or we could have a single method and define a base state that all states must inherit from and to have this base
    // state only used for primitive values which we handle directly, other values should use the writer-specific state?
    // Sounds a bit convoluted, needs more thought.
    public required Func<TDeclaringType, ODataJsonWriterState, ValueTask> WriteValue { get; init; }
}
