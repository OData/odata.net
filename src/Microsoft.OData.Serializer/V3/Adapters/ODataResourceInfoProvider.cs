using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal class ODataResourceInfoProvider : IODataResourceInfoProvider
{
    private Dictionary<Type, object> typeInfoMap = [];

    public ODataResourceTypeInfo<T> GetResourceInfo<T>()
    {
        var typeInfo = TryGetResourceInfo<T>();
        if (typeInfo is null)
        {
            throw new Exception($"Could not find ODataResourceTypeInfo for CLR type {typeof(T).FullName}.");
        }

        return typeInfo;
    }

    public ODataResourceTypeInfo<T>? TryGetResourceInfo<T>()
    {
        if (typeInfoMap.TryGetValue(typeof(T), out var typeInfo))
        {
            return (ODataResourceTypeInfo<T>)typeInfo;
        }

        return null;
    }

    internal void AddResourceInfo<T>(ODataResourceTypeInfo<T> resourceTypeInfo)
    {
        this.typeInfoMap[typeof(T)] = resourceTypeInfo;
    }
}
