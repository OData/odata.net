using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter;

internal interface IResourcePropertyWriter<TResource, TProperty>
{
    ValueTask WriteProperty(TResource resource, TProperty property, ODataWriterState state);
    ValueTask WriteDynamicProperty(
        TResource resource,
        string propertyName,
        object propertyValue,
        IEdmTypeReference propertyType,
        ODataWriterState state);
}
