//---------------------------------------------------------------------
// <copyright file="IQueryValueVisitor`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    /// <summary>
    /// Visitor for the QueryValue type hierarchy.
    /// </summary>
    /// <typeparam name="TResult">The type which the visitor will return.</typeparam>
    public interface IQueryValueVisitor<TResult>
    {
        /// <summary>
        /// Visits the given QueryValue
        /// </summary>
        /// <param name="value">QueryValue being visited.</param>
        /// <returns>The result of visiting this QueryValue.</returns>
        TResult Visit(QueryCollectionValue value);

        /// <summary>
        /// Visits the given QueryValue
        /// </summary>
        /// <param name="value">QueryValue being visited.</param>
        /// <returns>The result of visiting this QueryValue.</returns>
        TResult Visit(QueryRecordValue value);

        /// <summary>
        /// Visits the given QueryValue
        /// </summary>
        /// <param name="value">QueryValue being visited.</param>
        /// <returns>The result of visiting this QueryValue.</returns>
        TResult Visit(QueryReferenceValue value);

        /// <summary>
        /// Visits the given QueryValue
        /// </summary>
        /// <param name="value">QueryValue being visited.</param>
        /// <returns>The result of visiting this QueryValue.</returns>
        TResult Visit(QueryScalarValue value);

        /// <summary>
        /// Visits the given QueryValue
        /// </summary>
        /// <param name="value">QueryValue being visited.</param>
        /// <returns>The result of visiting this QueryValue.</returns>
        TResult Visit(QueryStructuralValue value);
    }
}