using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal interface IAnnotationsWriterProvider
{
    // Should the provider take the value or the type?
    public IAnnotationsWriter GetAnnotationsWriter(object value, ODataWriterContext context);
    public IAnnotationsWriter<T> GetAnnotationsWriter<T>(T value, ODataWriterContext context);
}
