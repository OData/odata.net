using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal interface IODataValueWriterProvider
{
    /// <summary>
    /// Gets a value writer for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <returns>An instance of <see cref="IODataWriter{T}"/> for the specified type.</returns>
    IODataWriter<T> GetValueWriter<T>(IEdmType edmType, ODataWriterContext context);

    // Susceptible to boxing, we should avoid this if possible
    /// <summary>
    /// 
    /// </summary>
    /// <param name="edmType"></param>
    /// <returns></returns>
    IODataWriter GetValueWriter(IEdmType edmType, Type valueType, ODataWriterContext context);
}
