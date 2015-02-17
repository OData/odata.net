//---------------------------------------------------------------------
// <copyright file="IQueryValueDeepCopyingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for creating deep copies of query values
    /// </summary>
    [ImplementationSelector("QueryValueDeepCopyingVisitor", DefaultImplementation = "Default")]
    public interface IQueryValueDeepCopyingVisitor : IQueryValueVisitor<QueryValue>
    {
        /// <summary>
        /// Performs a deep copy on the given value and returns the copy
        /// </summary>
        /// <typeparam name="TValue">The type of the value and copy</typeparam>
        /// <param name="value">The value to copy</param>
        /// <returns>The deep-copy of the value</returns>
        TValue PerformDeepCopy<TValue>(TValue value) where TValue : QueryValue;
    }
}