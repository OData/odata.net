using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Core.NewWriter2;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
public readonly struct ODataJsonWriterStackFrame
{
    public SelectExpandClause SelectExpandClause { get; init; }
    public IEdmType EdmType { get; init; }
}
