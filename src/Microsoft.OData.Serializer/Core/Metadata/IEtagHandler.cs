using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface IEtagHandler<TContext, TState, TValue>
{
    public bool HasEtagValue(TValue value, TState state, TContext context, out string etagValue);
    public void WriteEtagValue(TValue value, TState state, TContext context);

}
