using Microsoft.OData.Core.NewWriter2.Core.Resource;
using Microsoft.OData.Core.NewWriter2.Json;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class EdmPropertyValueJsonWriterProvider : IPropertyValueWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty>
{

    public IPropertyValueWriter<ODataJsonWriterContext, ODataJsonWriterStack, TResource, IEdmProperty> GetPropertyValueWriter<TResource>(
        TResource resource,
        IEdmProperty property,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        throw new NotImplementedException();
    }
}
