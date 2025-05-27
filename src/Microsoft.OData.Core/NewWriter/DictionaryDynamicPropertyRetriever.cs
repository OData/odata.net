using Microsoft.OData.Edm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal class DictionaryDynamicPropertyRetriever<T> : IDynamicPropertiesRetriever, IDynamicPropertiesRetriever<T>
{
    ConcurrentDictionary<Type, PropertyInfo> _dynamicPropertiesDictCache = new ConcurrentDictionary<Type, PropertyInfo>();
    public IEnumerable<(string PropertyName, IEdmTypeReference PropertyType, object Value)> GetDynamicProperties(object value, ODataWriterState state)
    {
        var dictProperty = GetDynamicPropertiesDictProperty(value.GetType(), state.WriterContext);
        var propertiesDict = dictProperty.GetValue(value) as IDictionary<string, object>;

        foreach (var kvp in propertiesDict)
        {
            yield return (kvp.Key, null, kvp.Value);
        }
    }

    public IEnumerable<(string PropertyName, IEdmTypeReference PropertyType, object Value)> GetDynamicProperties(T value, ODataWriterState state)
    {
        var dictProperty = GetDynamicPropertiesDictProperty(typeof(T), state.WriterContext);
        var propertiesDict = dictProperty.GetValue(value) as IDictionary<string, object>;

        foreach (var kvp in propertiesDict)
        {
            var edmType = state.WriterContext.EdmTypeMapper.GetEdmType(value.GetType());
            yield return (kvp.Key, edmType, kvp.Value);
        }
    }

    protected virtual PropertyInfo GetDynamicPropertiesDictProperty(Type type, ODataWriterContext context)
    {
        if (_dynamicPropertiesDictCache.TryGetValue(type, out var cachedProperty))
        {
            return cachedProperty;
        }

        var dictType = typeof(IDictionary<,>).MakeGenericType(typeof(string), typeof(object));
        PropertyInfo dynamicPropertyDict = null;
        foreach (var property in type.GetProperties())
        {
            
            if (dictType.IsAssignableFrom(property.PropertyType))
            {
                dynamicPropertyDict = property;
                break;
            }
        }

        _dynamicPropertiesDictCache[type] = dynamicPropertyDict;

        return dynamicPropertyDict;
    }
}
