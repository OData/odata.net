using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal interface ICollectionCounter<TContext, TState, TValue, TProperty>
{
    /// <summary>
    /// Checks if the item <paramref name="value"/> has a count value to be written.
    /// If it's cheap to retrive the count value, it should return true and set <paramref name="count"/> to the count value.
    /// If the method returns true and <paramref name="count"/> is set, then the count value will be written to the output
    /// automatically without calling <see cref="WriteCountValue"/>. If the method returns true and <paramref name="count"/> is null,
    /// then <see cref="WriteCountValue"/> will be called to write the count value. The latter is suitable when it's expensive
    /// to extract the count value separately and cheaper to write it directly to the output. For example, if the count is
    /// stored as a JSON string instead of a number, it would be more efficient to write it directly to the output without
    /// decoding it to a number first.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="state"></param>
    /// <param name="context"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    bool HasCountValue(TValue value, TState state, TContext context, out long? count);

    /// <summary>
    /// Writes the count value to the underlying output. This method is only called if <see cref="HasCountValue"/> returns true
    /// and the count value returned by <see cref="HasCountValue"/> is null. This method is used when it's cheaper
    /// to write the count value directly than to compute it separately.
    /// <param name="value"></param>
    /// <param name="context"></param>
    /// <param name="state"></param>
    void WriteCountValue(TValue value, TState state, TContext context);

    bool HasNestedCountValue(TValue value, TProperty property, TState state, TContext context, out long? count);

    void WriteNestedCountValue(TValue value, TProperty property, TState state, TContext context);
}
