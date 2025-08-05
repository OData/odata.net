using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public class ODataPropertyInfo<TDeclaringType, TValue, TCustomState> : ODataPropertyInfo
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    public  Func<TDeclaringType, IValueWriter<TCustomState>, ODataJsonWriterState<TCustomState>, ValueTask> WriteValue { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, TValue>? GetValue { get; init; }

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
    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, long?>? GetCount { get; init; }

    public Func<TDeclaringType, ICountWriter<TCustomState>, ODataJsonWriterState<TCustomState>, ValueTask>? WriteCount { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, string?>? GetNextLink { get; init; }

    public Func<TDeclaringType, INextLinkWriter<TCustomState>, ODataJsonWriterState<TCustomState>, ValueTask>? WriteNextLink { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, bool>? ShouldSkip { get; init; }

}
