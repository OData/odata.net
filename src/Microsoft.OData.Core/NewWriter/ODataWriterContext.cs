using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Microsoft.OData.Core.NewWriter;

internal class ODataWriterContext
{
    public SelectExpandClause SelectExpandClause { get; set; }
    public IEdmModel Model { get; set; }
    public Utf8JsonWriter JsonWriter { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    public IODataValueWriterProvider WriterProvider { get; set; }

    public IDynamicPropertiesRetrieverProvider DynamicPropertiesRetrieverProvider { get; set; }

    public IEdmTypeMapper EdmTypeMapper { get; set; }
}
