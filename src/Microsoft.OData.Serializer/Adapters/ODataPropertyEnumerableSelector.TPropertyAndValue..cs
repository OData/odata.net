namespace Microsoft.OData.Serializer;

public class ODataPropertyEnumerableSelector<TResource, TProperty, TValue, TCustomState>
    : ODataPropertyEnumerableSelector<TResource, TProperty, TCustomState>
{
    public required Func<TResource, TProperty, ODataWriterState<TCustomState>, string>
        GetPropertyName { get; set; }

    public required Func<TResource, TProperty, ODataWriterState<TCustomState>, TValue>
        GetPropertyValue { get; set; }

    protected override bool WritePropertyImplementation(TResource resource, TProperty propertyItem, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        if (state.Stack.Current.PropertyProgress < PropertyProgress.Name)
        {
            var propertyName = GetPropertyName(resource, propertyItem, state);
            jsonWriter.WritePropertyName(propertyName);
            state.Stack.Current.PropertyProgress = PropertyProgress.Name;
        }

        var value = GetPropertyValue(resource, propertyItem, state);
        return state.WriteValue(value);
    }
}
