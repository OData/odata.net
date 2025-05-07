using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal class ODataWriterState
{
    public ODataWriterContext WriterContext { get; set; }
    public IEdmType EdmType { get; set; }
}
