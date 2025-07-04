using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface IMetadataWriter<TContext, TState, TValue, TProperty>
{
    
    ValueTask WriteContextUrlAsync(TValue value, TState state, TContext context);
    ValueTask WriteCountPropertyAsync(TValue value, TState state, TContext context);
    ValueTask WriteNextLinkPropertyAsync(TValue value, TState state, TContext context);

    ValueTask WriteEtagPropertyAsync(TValue value, TState state, TContext context);

    ValueTask WriteNestedCountPropertyAsync(TValue value, TProperty resourceProperty, TState state, TContext context);
    ValueTask WriteNestedNextLinkPropertyAsync(TValue value, TProperty resourceProperty, TState state, TContext context);
}
