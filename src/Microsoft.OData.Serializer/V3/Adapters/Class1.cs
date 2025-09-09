using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class DefaultODataTypeMapper : IODataTypeMapper
{
    public virtual IEdmType? GetEdmType(Type type, IEdmModel model)
    {
        // TODO: support for attributes to specify type mapping
        IEdmType edmType = model.FindType(type.FullName);
        if (edmType != null)
        {
            return edmType;
        }

        // TODO? should we fallback to linear search for type with unqualified name?
        // I think that's not good for perf. Users can override this method if they want that behavior.

        return null;
    }
}
