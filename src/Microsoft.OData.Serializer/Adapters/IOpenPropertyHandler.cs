
namespace Microsoft.OData.Serializer;

internal interface IOpenPropertyHandler<TCustomState>
{
    // TODO: support resumability for dynamic properties
    void WriteOpenProperty<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state);

    void WriteOpenProperty<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state);
}
