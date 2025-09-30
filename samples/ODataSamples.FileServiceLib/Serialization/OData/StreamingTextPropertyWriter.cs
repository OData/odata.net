using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Streaming;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Serialization.OData;

/// <summary>
/// Handles serialization of streamable properties on
/// supported StreamingEnabled resource types.
/// It should be used with OData's <see cref="ODataPropertyValueWriterAttribute"/>.
/// </summary>
/// <typeparam name="TEntity">The resource type, it should inherit from <see cref="StreamingEnabled"/>.</typeparam>
/// <typeparam name="TValue"></typeparam>
internal class StreamingTextPropertyWriter<TEntity, TValue>
    : ODataAsyncPropertyWriter<TEntity, TValue, ODataCustomState>
    where TEntity : StreamingEnabled
{
    public override async ValueTask WriteValueAsync(
        TEntity resource,
        TValue propertyValue,
        IStreamValueWriter<ODataCustomState> writer,
        ODataWriterState<ODataCustomState> state)
    {
        ODataPropertyInfo? property = state.CurrentPropertyInfo();
        Debug.Assert(property != null && property.Name != null);

        if (resource.StreamableProperties != null && resource.StreamableProperties.TryGetValue(property.Name, out var streamingSource))
        {
            await StreamingSourceWriter.WriteTextStreamAsync(streamingSource, writer, state);
            return;
        }

        await writer.WriteValueAsync(propertyValue, state);
    }
}
