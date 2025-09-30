
using Microsoft.OData.Edm;

namespace Microsoft.OData.Serializer;

internal class ODataJsonByteArrayWriter<TCustomState> : ODataJsonWriter<byte[], TCustomState>
{
    public override bool Write(byte[] value, ODataWriterState<TCustomState> state)
    {
        // TODO: byte[] could either be Edm.Binary or Collection(Edm.Byte). Currently,
        // this only assumes Edm.Binary. But we should also support Collection(Edm.Byte).

        // Base64 encoding expands length by 4/3
        // We use a fixed threshold here to ensure that the condition always returns the same result
        // for the same input value. Ideally, we should write value if it can fit the buffer capacity,
        // but if's possibly that the buffer capacity changes between calls such that the if statement
        // is not taken the first iteration, but taken the second and lead to invalid values.
        // Perhaps we should hold some state that tells us whether we are in chunking mode or not.
        if (value.Length * 4.0 / 3 < 16 * 1024)
        {
            state.JsonWriter.WriteBase64StringValue(value);
            return true;
        }

        if (state.ShouldFlush())
        {
            return false;
        }

        state.Stack.Push();
        bool success = LargeBinaryStringWriter<TCustomState>.WriteNextChunkFromByteArray(value, state);

        // In .NET 10, we can leverage Utf8JsonWriter.WriteBase64ValueSegment
        // to write chunks. But in < .NET 10, we have to handle
        // that manually.


        state.Stack.Pop(success);

        if (success)
        {
            state.JsonWriter.SignalWriteThatBypassedWriter();
        }

        return success;
    }
}
