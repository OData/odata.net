//---------------------------------------------------------------------
// <copyright file="IQueryTypeVisitor`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    /// <summary>
    /// Visitor for the QueryType hierarchy.
    /// </summary>
    /// <typeparam name="TResult">The result type which the visitor will return.</typeparam>
    public interface IQueryTypeVisitor<TResult>
    {
        /// <summary>
        /// Visits a <see cref="QueryAnonymousStructuralType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryAnonymousStructuralType type);

        /// <summary>
        /// Visits a <see cref="QueryClrEnumType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryClrEnumType type);

        /// <summary>
        /// Visits a <see cref="QueryClrPrimitiveType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryClrPrimitiveType type);

        /// <summary>
        /// Visits a <see cref="QueryCollectionType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryCollectionType type);

        /// <summary>
        /// Visits a <see cref="QueryComplexType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryComplexType type);

        /// <summary>
        /// Visits a <see cref="QueryEntityType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryEntityType type);

        /// <summary>
        /// Visits a <see cref="QueryGroupingType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryGroupingType type);

        /// <summary>
        /// Visits a <see cref="QueryMappedScalarType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryMappedScalarType type);

        /// <summary>
        /// Visits a <see cref="QueryMappedScalarTypeWithStructure"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryMappedScalarTypeWithStructure type);

        /// <summary>
        /// Visits a <see cref="QueryRecordType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryRecordType type);

        /// <summary>
        /// Visits a <see cref="QueryReferenceType"/>.
        /// </summary>
        /// <param name="type">Query type being visited.</param>
        /// <returns>The result of visiting this query type.</returns>
        TResult Visit(QueryReferenceType type);
    }
}
