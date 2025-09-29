using Microsoft.OData;
using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Api;
using ODataSamples.FileServiceLib.Models;
using ODataSamples.FileServiceLib.Schema;
using ODataSamples.FileServiceLib.Schema.Abstractions;
using ISchematizedFileItem = ODataSamples.FileServiceLib.Schema.Abstractions.ISchematizedObject<ODataSamples.FileServiceLib.Schema.Common.IFileItemSchema>;

namespace ODataSamples.FileServiceLib.Serialization.OData;

public static class ODataSerializerOptionsFactory
{
    public static ODataSerializerOptions<ODataCustomState> Create()
    {
        var options = new ODataSerializerOptions<ODataCustomState>();
        MapFindFileResponse(options);
        MapFileItem(options);
        MapOpenPropertyValue(options);
        return options;
    }

    private static void MapFindFileResponse(ODataSerializerOptions<ODataCustomState> options)
    {
        options.AddTypeInfo<FindFileResponse>(new()
        {
            GetCount = (response, state) => response.Count,
            GetNextLink = (response, state) =>
            {
                if (!string.IsNullOrEmpty(response.SkipToken))
                {
                    // TODO: not a good idea to modify the uri from there
                    state.ODataUri.SkipToken = response.SkipToken;
                    return state.ODataUri.BuildUri(ODataUrlKeyDelimiter.Slash).AbsoluteUri;
                }

                return null;
            },
            // Places the next link after the value array
            NextLinkPosition = AnnotationPosition.PostValue
        });
    }

    private static void MapFileItem(ODataSerializerOptions<ODataCustomState> options)
    {
        options.AddTypeInfo<ISchematizedFileItem>(new()
        {
            EdmTypeName = $"{SchemaConstants.Namespace}.FileItem",
            GetODataId = (item, state) => {
                if (item is SchematizedFileItem fileItem)
                {
                    return state.CustomState.IdSerializer?.GetODataId(fileItem).AbsoluteUri;
                }

                return null;
            },
            GetEtag = (item, state) =>
            {
                if (item is SchematizedFileItem fileItem)
                {
                    return fileItem.GetEtag();
                }

                return null;
            },

            PropertySelector = new ODataPropertyEnumerableSelector<ISchematizedFileItem, KeyValuePair<IPropertyDefinition, object>, ODataCustomState>()
            {
                GetProperties = (item, state) => item.Data,
                WriteProperty = (item, property, writer, state) => writer.WriteProperty(property.Key.Name, property.Value, state)
            }
        });
    }

    private static void MapOpenPropertyValue(ODataSerializerOptions<ODataCustomState> options)
    {
        options.AddTypeInfo<IOpenPropertyValue>(new()
        {
            GetEdmTypeName = (item, state) => $"{SchemaConstants.Namespace}.{item.GetType().Name}",
            GetDynamicProperties = (item, state) => item.GetAllOpenProperties(),
            GetPropertyPreValueAnnotations = (item, propertyName, state) => item.GetODataAnnotations(propertyName)
        });
    }
}
