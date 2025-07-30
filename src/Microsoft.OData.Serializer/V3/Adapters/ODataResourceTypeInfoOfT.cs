using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataResourceTypeInfo<T> : ODataResourceTypeInfo
{
    public override Type Type { get; init; } = typeof(T);

    public IReadOnlyList<ODataPropertyInfo<T>> Properties { get; init; }

    public Func<T, ODataJsonWriterState, bool>? HasCount { get; init; }
    public Func<T, ODataJsonWriterState, ValueTask>? WriteCount { get; init; }

    public Func<T, ODataJsonWriterState, bool>? HasNextLink { get; init; }

    public Func<T, ODataJsonWriterState, ValueTask>? WriteNextLink { get; init; }

    public Func<T, ODataJsonWriterState, bool>? HasEtag { get; init; }

    public Func<T, ODataJsonWriterState, ValueTask>? WriteEtag { get; init; }

}
