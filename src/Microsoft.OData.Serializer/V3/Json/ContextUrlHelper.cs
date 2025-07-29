using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json;

internal static class ContextUrlHelper
{
    public static void WriteContextUrlProperty(ODataPayloadKind payloadKind, ODataJsonWriterState state)
    {
        switch (payloadKind)
        {
            case ODataPayloadKind.ResourceSet:
                WriteResourceSetContextUrl(state);
                break;

            default:
                // No context URL for other payload kinds
                throw new NotImplementedException();
        }
    }

    internal static void WriteResourceSetContextUrl(ODataJsonWriterState state)
    {
        if (state.ODataUri == null)
        {
            return;
        }

        var odataUri = state.ODataUri;
        var writer = state.JsonWriter;

        // TODO: This is a naive, incorrect, incomplete implementation of the context URL.
        // It's just a placeholder to show how it might be constructed.

        // TODO for perf, we could replace StringBuilder with custom builder that avoids allocations via stackalloc array or pooled array
        // and uses utf-8 encoding directly for smaller memory footprint and reduced need for
        StringBuilder uri = new();
        uri.Append(odataUri.ServiceRoot?.AbsoluteUri);
        uri.Append("$metadata#");

        if (odataUri.Path.Count > 0)
        {
            uri.Append(odataUri.Path[0].Identifier);
        }

        for (int i = 1; i < odataUri.Path.Count; i++)
        {
            uri.Append('/');
            uri.Append(odataUri.Path[i].Identifier);
        }

        writer.WriteString("@odata.context", uri.ToString());
    }
}
