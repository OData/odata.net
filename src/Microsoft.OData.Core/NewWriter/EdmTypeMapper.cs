using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal class EdmTypeMapper : IEdmTypeMapper
{
    public IEdmTypeReference GetEdmType(Type type)
    {
        if (type.IsAssignableFrom(typeof(IEnumerable)))
        {
            return new EdmCollectionTypeReference(
                new EdmCollectionType(new EdmUntypedTypeReference(EdmCoreModel.Instance.GetUntypedType())));
        }

        // throw new NotImplementedException($"Not yet implemented GetEdmType() for type {type.Name}");
        return new EdmUntypedTypeReference(EdmCoreModel.Instance.GetUntypedType());
    }
}
