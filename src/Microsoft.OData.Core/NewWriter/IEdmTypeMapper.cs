using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal interface IEdmTypeMapper
{
    IEdmTypeReference GetEdmType(Type type);
}
