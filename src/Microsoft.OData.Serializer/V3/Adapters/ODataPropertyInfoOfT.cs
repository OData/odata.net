using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataPropertyInfo<TDeclaringType, TCustomState> : ODataPropertyInfo
{
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
    public required Func<TDeclaringType, ODataJsonWriterState<TCustomState>, ValueTask> WriteValue { get; init; }

    // TODO: Should nested count handlers be defined on the property info or the type info?
    // We already have count handlers on the type info, keeping them there would provide
    // uniformit. It might be cause of confusion to have to set it multiple places.
    // The challenge with having it on the type info is that when comes to writing
    // nested annotations, e.g. PropertyName@odata.count, we need to write this before
    // the value. We need to write this annotation before the value, so at this time
    // we have not resolved type info and we don't know what annotation handlers
    // it exposes. But we have access to the property info. Having it on the
    // property info also makes it convenient to add annotations to primitive properties
    // without having to define type infos for primitive values.
    // The other approach is to have them in the type infos, which means when writing
    // the property, we don't know what annotations are available until we resolve
    // the property value's writer. So the value writer would need to handle
    // writing property names, meaning each IODataWriter will need to check
    // the state to see if there's a property in scope, or if it's top-level, etc.
    // there might a cost to performing these if-checks on the hot path, but not sure it's significant.
    // It simplifies the configuration for the end user, the annotations are always on the type info.
    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, bool>? HasCount { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, ValueTask>? WriteCount { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, bool>? HasNextLink { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, ValueTask>? WriteNextLink { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, bool>? ShouldSkip { get; init; }
}
