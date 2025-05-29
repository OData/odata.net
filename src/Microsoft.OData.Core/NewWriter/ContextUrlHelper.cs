using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal static class ContextUrlHelper
{
    public static void WriteContextUrl(ODataPayloadKind payloadKind, ODataWriterState state)
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

    private static void WriteResourceSetContextUrl(ODataWriterState state)
    {
        if (state.WriterContext.ODataUri == null)
        {
            return;
        }

        var odataUri = state.WriterContext.ODataUri;
        var writer = state.WriterContext.JsonWriter;
        var type = state.EdmType;

        // TODO: This is a naive, incorrect, incomplete implementation of the context URL.
        // It's just a placeholder to show how it might be constructed.
        StringBuilder uri = new();
        uri.Append(odataUri.ServiceRoot?.AbsoluteUri);
        uri.Append("$metadata#");

        if (odataUri.Path.Segments.Count > 0)
        {
            uri.Append(odataUri.Path.Segments[0].Identifier);
        }

        for (int i = 1; i < odataUri.Path.Segments.Count; i++)
        {
            uri.Append("/");
            uri.Append(odataUri.Path.Segments[i].Identifier);
        }

        writer.WriteString("@odata.context", uri.ToString());
    }
}
