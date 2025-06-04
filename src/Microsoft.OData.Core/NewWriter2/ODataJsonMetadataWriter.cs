using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataJsonMetadataWriter<TValue>(
    ICollectionCounterProvider<ODataJsonWriterContext, ODataJsonWriterStack> counterProvider)
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
        var counter = counterProvider.GetCounter<TValue>(context, state);
        if (counter.TryGetCount(value, context, state, out long count))
        {
            
            context.JsonWriter.WritePropertyName("@odata.count");
            
            context.JsonWriter.WriteNumberValue(count);
            
        }

        return ValueTask.CompletedTask;
    }
}
