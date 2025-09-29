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
                if (item is FileItem fileItem)
                {
                    return state.CustomState.IdSerializer?.GetODataId(fileItem).AbsoluteUri;
                }

                return null;
            },
            GetEtag = (item, state) =>
            {
                if (item is FileItem fileItem)
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
        // Here we use the concrete type instead of the IOpenPropertyValue interface because
        // the values in the fileItem.Data dictionary are of type object, so the serializer
        // doesn't know to use the IOpenPropertyValue mapping.
        // TODO: Need to provide a solution for this, would be too tedious to add a mapping for
        // each concrete type.
        options.AddTypeInfo<ExtensionOpenPropertyValue>(new()
        {
            GetEdmTypeName = (item, state) => $"{SchemaConstants.Namespace}.{item.GetType().Name}",
            GetOpenProperties = (item, state) => item.GetAllOpenProperties(),
            GetPropertyPreValueAnnotations = (item, propertyName, state) => item.GetODataAnnotations(propertyName)
        });

        // Here we use the interface IOpenPropertyValue to cover cases where the declared type that
        // the serializer sees is IOpenPropertyValue, such as when we use IOpenPropertyValue[] arrays.
        options.AddTypeInfo<IOpenPropertyValue>(new()
        {
            GetEdmTypeName = (item, state) => $"{SchemaConstants.Namespace}.{item.GetType().Name}",
            GetOpenProperties = (item, state) => item.GetAllOpenProperties(),
            GetPropertyPreValueAnnotations = (item, propertyName, state) => item.GetODataAnnotations(propertyName)
        });
    }
}
