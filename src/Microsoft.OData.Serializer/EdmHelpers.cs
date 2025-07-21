using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

internal static class EdmHelpers
{
    public static bool IsStructuredCollection(this IEdmTypeReference type)
    {
        if (type is IEdmCollectionType collectionType)
        {
            return collectionType.ElementType.IsStructured();
        }

        return false;
    }
}
