using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataJsonMetadataWriterProvider(
    ICollectionCounterProvider<ODataJsonWriterContext, ODataJsonWriterStack> counterProvider)
    : IMetadataWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack>
{
    public IMetadataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue> GetMetadataWriter<TValue>(ODataJsonWriterContext context, ODataJsonWriterStack state)
    {
        return new ODataJsonMetadataWriter<TValue>(counterProvider);
    }
}
