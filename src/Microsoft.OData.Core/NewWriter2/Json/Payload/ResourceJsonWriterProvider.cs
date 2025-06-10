using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class ResourceJsonWriterProvider : IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack>
{
    public IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue> GetResourceWriter<TValue>(ODataJsonWriterContext context, ODataJsonWriterStack state)
    {
        // TODO: should cache
        return new PocoResourceJsonWriter<TValue>();
    }
}
