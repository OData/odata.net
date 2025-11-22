using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public sealed class ODataElementAsyncEnumerableSelector<TResource, TElement>
    : ODataElementAsyncEnumerableSelector<TResource, TElement, DefaultState>
{
}
