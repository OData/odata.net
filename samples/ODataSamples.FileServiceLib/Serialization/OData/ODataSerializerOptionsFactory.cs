using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Models;
using ODataSamples.FileServiceLib.Schema.Abstractions;
using ODataSamples.FileServiceLib.Schema.Common;
using ISchematizedFileItem = ODataSamples.FileServiceLib.Schema.Abstractions.ISchematizedObject<ODataSamples.FileServiceLib.Schema.Common.IFileItemSchema>;

namespace ODataSamples.FileServiceLib.Serialization.OData;

public static class ODataSerializerOptionsFactory
{
    public static ODataSerializerOptions<ODataCustomState> Create()
    {
        var options = new ODataSerializerOptions<ODataCustomState>();
        AddFileItemTypeInfo(options);
        return options;
    }

    private static void AddFileItemTypeInfo(ODataSerializerOptions<ODataCustomState> options)
    {
        options.AddTypeInfo<ISchematizedFileItem>(new()
        {
            EdmTypeName = "OData.Samples.FileService.FileItem",
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
}
