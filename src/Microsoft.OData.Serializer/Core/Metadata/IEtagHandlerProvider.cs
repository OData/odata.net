using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Core;

public interface IEtagHandlerProvider<TContext, TState>
{
    /// <summary>
    /// Gets an ETag handler for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the value to handle.</typeparam>
    /// <returns>An instance of <see cref="IEtagHandler{TContext, TState, T}"/> for the specified type.</returns>
    IEtagHandler<TContext, TState, T> GetEtagHandler<T>(TState state, TContext context);
}
