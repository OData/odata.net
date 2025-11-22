using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public class ODataElementAsyncEnumerableSelector<TResource, TElement, TCustomState>
    : ODataElementSelector<TResource, TCustomState>
{
    public required Func<TResource, ODataWriterState<TCustomState>, IAsyncEnumerable<TElement>> GetElements { get; set; }

    public Func<TResource, TElement, IValueWriter<TCustomState>, ODataWriterState<TCustomState>, bool>? WriteElement { get; set; }

    internal override bool WriteElements(TResource resource, ODataWriterState<TCustomState> state)
    {
        throw new NotImplementedException();
    }
}
