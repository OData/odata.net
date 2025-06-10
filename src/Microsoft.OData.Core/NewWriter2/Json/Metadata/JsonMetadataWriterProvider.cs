using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class JsonMetadataWriterProvider(
    IMetadataValueProvider<ODataJsonWriterContext, ODataJsonWriterStack> metadataValueProvider)
    : IMetadataWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack>
{
    public IMetadataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue> GetMetadataWriter<TValue>(
        ODataJsonWriterContext context,
        ODataJsonWriterStack state)
    {
        // TODO: should be cached
        return new JsonMetadataWriter<TValue>(metadataValueProvider);
    }
}
