using Microsoft.OData.Core.NewWriter2.Core;
using Microsoft.OData.Core.NewWriter2.Core.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2.Json.Payload;

internal class ResourceJsonWriterProvider : IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack>
{
    public IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue> GetResourceWriter<TValue>(ODataJsonWriterContext context, ODataJsonWriterStack state)
    {
        // TODO: should cache
        return new PocoResourceJsonWriter<TValue>();
    }
}
