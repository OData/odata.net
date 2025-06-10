using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class PocoResourceJsonWriter<T> : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T>
{
    public async ValueTask WriteAsync(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        // TODO: handle null, should we check value == null? or should have an IsNull method
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteStartObject();


        // TODO: implement field selection, annotations, etc.
        if (context.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            // Write context URL if needed
            var metadataWriter = context.MetadataWriterProvider.GetMetadataWriter<T>(context, state);
            await metadataWriter.WriteEtagPropertyAsync(value, state, context);
        }

        // AspNetCoreOData serializes properties in a certain deterministic order: strucural properties first, the dynamic properties, etc.
        // However, according to the spec, the order is considered insignificant. So in this case we'll go with
        // arbitrary order for performance. But there might be cases where we want to have consistent order with AspNetCore
        // or other libraries, or when handling streaming scenarios where it may be beneficial to serialize
        // properties as they arrive from the source

        var edmType = state.Current.EdmType as IEdmStructuredType;
        var selectExpand = state.Current.SelectExpandClause;
        if (state.Current.SelectExpandClause == null || state.Current.SelectExpandClause.AllSelected)
        {
            // If all properties are selected, we can write all properties
            foreach (var property in edmType.StructuralProperties())
            {

            }

            foreach (var property in edmType.NavigationProperties())
            {

            }

            // TODO handle dynamic properties

        }
        else
        {
            foreach (var item in selectExpand.SelectedItems)
            {
                if (item is PathSelectItem pathSelectItem)
                {
                    
                }
                else if (item is ExpandedNavigationSelectItem)
                {

                }
                else if (item is ExpandedCountSelectItem)
                {

                }
            }
        }

        jsonWriter.WriteEndObject();
    }
}