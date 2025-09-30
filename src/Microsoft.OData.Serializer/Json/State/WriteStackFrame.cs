using System.Collections;

namespace Microsoft.OData.Serializer;

internal struct WriteStackFrame<TCustomState>
{
    public ODataTypeInfo? ResourceTypeInfo { get; set; }
    public ODataPropertyInfo? PropertyInfo { get; set; }

    public bool IsContinuation { get; set; }

    public ResourceWriteProgress ResourceProgress { get; set; }

    public PropertyProgress PropertyProgress { get; set; }

    public int EnumeratorIndex { get; set; }

    public IEnumerator CurrentEnumerator { get; set; }
    public ValueTask? PendingTask { get; set; }
}
