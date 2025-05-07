using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter;

internal interface IResourcePropertyWriter<TResource, TProperty>
{
    ValueTask WriteProperty(TResource resource, TProperty property, ODataWriterState context);
}
