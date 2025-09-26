using Microsoft.OData.Serializer.V3.Json;
using Microsoft.OData.Serializer.V3.Json.State;
using Microsoft.OData.Serializer.V3.Json.Writers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataPropertyEnumerableSelector<TResource, TProperty, TCustomState>
    : ODataPropertySelector<TResource, TCustomState>
{
    public required Func<TResource, ODataWriterState<TCustomState>, IEnumerable<TProperty>>
        GetProperties { get; set; }

    public Func<TResource, TProperty, IPropertyWriter<TCustomState>, ODataWriterState<TCustomState>, bool>?
        WriteProperty { get; set; }

    internal override bool WriteProperties(TResource resource, ODataWriterState<TCustomState> state)
    {
        IEnumerator<TProperty> enumerator;
        if (state.Stack.Current.CurrentEnumerator == null)
        {
            var properties = this.GetProperties(resource, state);
            enumerator = properties.GetEnumerator();
            state.Stack.Current.CurrentEnumerator = enumerator;
            if (!enumerator.MoveNext())
            {
                // If the collection is empty, we can return true immediately.
                return true;
            }
        }
        else
        {
            // Retrieve the enumerator from the state so we can resume writing from where we left off.
            enumerator = (state.Stack.Current.CurrentEnumerator as IEnumerator<TProperty>)!;
            Debug.Assert(enumerator != null, "CurrentEnumerator should be of type IEnumerator<TProperty>. Possible bug in state management and property resume operation.");
        }

        do
        {
            if (state.ShouldFlush())
            {
                return false;
            }

            var item = enumerator.Current;
            if (!this.WritePropertyImplementation(resource, item, state))
            {
                return false;
            }

            state.Stack.EndProperty();

        } while (enumerator.MoveNext());

        return true;
    }

    protected virtual bool WritePropertyImplementation(TResource resource, TProperty propertyItem, ODataWriterState<TCustomState> state)
    {
        Debug.Assert(WriteProperty != null, "WriteProperty should not be null if this method is called.");

        return this.WriteProperty(resource, propertyItem, DefaultPropertyWriter<TCustomState>.Instance, state);
    }
}
