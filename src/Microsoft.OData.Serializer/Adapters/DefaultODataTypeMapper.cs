using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

public class DefaultODataTypeMapper : IODataTypeMapper
{
    public virtual IEdmType? GetEdmType(Type type, IEdmModel model)
    {
        IEdmType? edmType = GetTypeFromAttribute(type, model);
        if (edmType != null)
        {
            return edmType;
        }

        edmType = model.FindType(type.FullName);
        if (edmType != null)
        {
            return edmType;
        }

        // TODO? should we fallback to linear search for type with unqualified name?
        // I think that's not good for perf. Users can override this method if they want that behavior.

        return null;
    }

    private static IEdmType? GetTypeFromAttribute(Type type, IEdmModel model)
    {
        var attribute = type.GetCustomAttribute<ODataTypeAttribute>(inherit: false); // TODO: Should we support inheriting the attribute?
        if (attribute != null)
        {
            var edmType = model.FindType(attribute.FullTypeName)
                ?? throw new Exception($"Could not find EDM type '{attribute.FullTypeName}' specified in ODataTypeAttribute on CLR type '{type.FullName}'");
            return edmType;
        }

        return null;
    }
}
