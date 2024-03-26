using Microsoft.OData;
using Microsoft.OData.Edm;
using System.Text.Json;

namespace ExperimentsLib
{
    public static class OptionsHelper
    {
        public static JsonSerializerOptions CreateJsonSerializerOptions(IEdmModel model, ODataUri uri)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            //options.Converters.Add(new CustomerConverter());
            ODataSerializerContext context = new ODataSerializerContext()
            {
                Model = model,
                Uri = uri
            };
            options.Converters.Add(new ODataJsonConverterFactory(context));
            return options;
        }
    }
}