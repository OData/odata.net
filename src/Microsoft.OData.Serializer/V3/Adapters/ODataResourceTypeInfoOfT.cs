using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataResourceTypeInfo<T, TCustomState> : ODataResourceTypeInfo
{
    public override Type Type { get; init; } = typeof(T);

    public IReadOnlyList<ODataPropertyInfo<T, TCustomState>> Properties { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, bool>? HasCount { get; init; }
    public Func<T, ODataJsonWriterState<TCustomState>, ValueTask>? WriteCount { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, bool>? HasNextLink { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, ValueTask>? WriteNextLink { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, bool>? HasEtag { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, ValueTask>? WriteEtag { get; init; }

}
