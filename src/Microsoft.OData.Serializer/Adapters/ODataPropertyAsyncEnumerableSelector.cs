using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer
{
    public class ODataPropertyAsyncEnumerableSelector<TResource, TProperty, TCustomState>
        : ODataPropertySelector<TResource, TCustomState>
    {
        internal ODataPropertyAsyncEnumerableSelector() { } // prevent

        public required Func<TResource, ODataWriterState<TCustomState>, IAsyncEnumerable<TProperty>>
        GetProperties
        { get; set; }

        public Func<TResource, TProperty, IPropertyWriter<TCustomState>, ODataWriterState<TCustomState>, bool>?
            WriteProperty
        { get; set; }
        internal override bool WriteProperties(TResource resource, ODataWriterState<TCustomState> state)
        {
            throw new NotImplementedException();
        }
    }
}
