using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface IMetadataWriterProvider<TContext, TState, TProperty>
{
    IMetadataWriter<TContext, TState, TValue, TProperty> GetMetadataWriter<TValue>(TContext context, TState state);
}
