using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Core.NewWriter2;

internal readonly struct ODataJsonWriterStackFrame
{
    public SelectExpandClause SelectExpandClause { get; init; }
    public IEdmType EdmType { get; init; }
}
