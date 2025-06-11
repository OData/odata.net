using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class JsonMetadataWriter<TValue>(
    IMetadataValueProvider<ODataJsonWriterContext, ODataJsonWriterStack> metadataValueProvider)
    : IMetadataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>
{
    /// <summary>
    /// See:
    /// - https://docs.oasis-open.org/odata/odata-json-format/v4.0/errata03/os/odata-json-format-v4.0-errata03-os-complete.html#_The_odata.metadata_Annotation_1
    /// - https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControlInformationcontextodatacontex
    /// - https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#_Toc31358910
    /// -
    /// </summary>
    /// <param name="context"></param>
    /// <param name="state"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ValueTask WriteContextUrlAsync(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        if (context.PayloadKind == ODataPayloadKind.ResourceSet && state.IsTopLevel())
        {
            ContextUrlHelper.WriteContextUrlProperty(context.PayloadKind, context, state);
            return ValueTask.CompletedTask;
        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// See:
    /// - https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControlInformationcountodatacount
    /// - https://docs.oasis-open.org/odata/odata-json-format/v4.0/errata03/os/odata-json-format-v4.0-errata03-os-complete.html#_Toc453766631
    /// </summary>
    /// <param name="context"></param>
    /// <param name="state"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ValueTask WriteCountPropertyAsync(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var counter = metadataValueProvider.GetCounter<TValue>(context, state);
        if (counter.HasCountValue(value, state, context, out long? count))
        {

            context.JsonWriter.WritePropertyName("@odata.count"u8);

            if (count.HasValue)
            {
                context.JsonWriter.WriteNumberValue(count.Value);
            }
            else
            {
                counter.WriteCountValue(value, state, context);
            }
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask WriteNextLinkPropertyAsync(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var nextLinkRetriever = metadataValueProvider.GetNextLinkHandler<TValue>(state, context);
        if (nextLinkRetriever.HasNextLinkValue(value, state, context, out Uri nextLink))
        {
            context.JsonWriter.WritePropertyName("@odata.nextLink"u8);
            if (nextLink is not null)
            {
                context.JsonWriter.WriteStringValue(nextLink.ToString());
            }
            else
            {
                nextLinkRetriever.WriteNextLinkValue(value, state, context);
            }
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask WriteEtagPropertyAsync(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var etagHandler = metadataValueProvider.GetEtagHandler<TValue>(state, context);
        if (etagHandler.HasEtagValue(value, state, context, out string etagValue))
        {
            context.JsonWriter.WritePropertyName("@odata.etag"u8);
            context.JsonWriter.WriteStringValue(etagValue);
        }

        return ValueTask.CompletedTask;
    }
}
