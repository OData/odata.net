using Microsoft.OData.Edm;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal static class ODataSerializer
{
    public static async ValueTask WriteAsync<T>(T value, Stream stream, ODataUri uri, IEdmModel model, ODataSerializerOptions options)
    {
        var payloadKind = GetPayloadKind(uri, model);
        switch (payloadKind)
        {
            case ODataPayloadKind.ResourceSet:
                await WriteResourceSetAsync(value, stream, uri, model, options).ConfigureAwait(false);
                break;
            default:
                throw new NotSupportedException($"Unsupported payload kind: {payloadKind}");
        }

    }

    private async static ValueTask WriteResourceSetAsync<T>(T value, Stream stream, ODataUri uri, IEdmModel model, ODataSerializerOptions options)
    {
        var jsonWriter = new Utf8JsonWriter(stream);
        var context = new ODataJsonWriterContext
        {
            ODataUri = uri,
            Model = model,
            MetadataLevel = options.MetadataLevel,
            PayloadKind = GetPayloadKind(uri, model),
            ODataVersion = options.ODataVersion,
            JsonSerializerOptions = options.JsonSerializerOptions,
            JsonWriter = jsonWriter,
            ValueWriterProvider = options.ValueWriterProvider,
            ResourcePropertyWriterProvider = options.ResourcePropertyWriterProvider,
            MetadataWriterProvider = options.MetadataWriterProvider,
            PropertyValueWriterProvider = options.PropertyValueWriterProvider
        };


        var writerStack = new ODataJsonWriterStack();
        writerStack.Push(new ODataJsonWriterStackFrame
        {
            EdmType = new EdmCollectionType(
                new EdmEntityTypeReference(
                    model.FindType("ns.Customer") as IEdmEntityType, isNullable: false)),
            SelectExpandClause = uri.SelectAndExpand
        });

        var odataWriter = context.GetValueWriter<T>(writerStack);
        await odataWriter.WriteAsync(value, writerStack, context);

        // TODO: should we guarantee flushing from within the writer?
        await jsonWriter.FlushAsync();
    }

    private static ODataPayloadKind GetPayloadKind(ODataUri uri, IEdmModel model)
    {
        // TODO compute payload kind
        return ODataPayloadKind.ResourceSet;
    }
}
