using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

internal class ODataJsonHybridValueKindWriter<T, TCustomState>(ODataTypeInfo<T, TCustomState> typeInfo) :
    ODataJsonWriter<T, TCustomState>
{
    // Since this cache is only attached to this writer instance, no need to make it concurrent.
    private readonly Dictionary<ODataValueKind, ODataWriter<T, ODataWriterState<TCustomState>>> _writerCache = [];

    public override bool Write(T value, ODataWriterState<TCustomState> state)
    {
        Debug.Assert(typeInfo.GetValueKind != null, "typeInfo.GetValueKind must not be null in ODataJsonHybridValueKindWriter.");
        var valueKind = typeInfo.GetValueKind(value, state);
        var writer = GetWriterForValueKind(valueKind);
        return writer.Write(value, state);
    }

    private ODataWriter<T, ODataWriterState<TCustomState>> GetWriterForValueKind(ODataValueKind valueKind)
    {
        if (_writerCache.TryGetValue(valueKind, out var writer))
        {
            return writer;
        }

        writer = valueKind switch
        {
            ODataValueKind.Resource => new ODataResourceJsonWriter<T, TCustomState>(typeInfo),
            ODataValueKind.Collection => new ODataJsonResourceSetWithElementSelectorWriter<T, TCustomState>(typeInfo),
            _ => throw new Exception($"Unsupported writer for value kind {valueKind} for type '{typeof(T).FullName}")
        };
        
        _writerCache[valueKind] = writer;
        return writer;
    }
}
