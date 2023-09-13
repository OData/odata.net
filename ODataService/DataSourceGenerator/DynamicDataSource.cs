using Microsoft.Restier.Core;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DataSourceGenerator
{
    public class DynamicDataSource : ApiBase
    {
        static MethodInfo deserializeMethod = typeof(JsonSerializer).GetMethod("Deserialize", new Type[] { typeof(JsonNode), typeof(JsonSerializerOptions) });
        public DynamicDataSource(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public void Load(string json)
        {
            Type dataSourceType = this.GetType();
            var jsonObject = JsonObject.Parse(json) as JsonObject;
            if (jsonObject != null)
            {
                foreach (var item in jsonObject)
                {
                    var prop = dataSourceType.GetProperty(item.Key);
                    if (prop != null)
                    {
                        try
                        {
                            var value = Create(item.Value, prop.PropertyType);
                            prop.SetValue(this, value);
                        }
                        catch { }   
                    }
                }
            }
        }

        public object Create(JsonNode json, Type type)
        {
            var deserialize = deserializeMethod.MakeGenericMethod(type);
            return deserialize.Invoke(null, new object[] { json, null });
        }
    }
}
