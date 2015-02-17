//---------------------------------------------------------------------
// <copyright file="QueryClrCollectionTypeFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A factory to create instances of QueryClrCollectionTypes
    /// </summary>
    public static class QueryClrCollectionTypeFactory
    {
        /// <summary>
        /// Creates a QueryCollectionType which implements IQueryClrType so that codegen creates the right list type
        /// </summary>
        /// <typeparam name="TQueryType">The element type of the Collection</typeparam>
        /// <param name="queryType">The query type of the Collection</param>
        /// <param name="clrTypeDefinition">The generic type definition of the collection type, e.g.: List&lt;&gt; </param>
        /// <returns>A QueryClrCollectionType which is used as a hint by Code Generation</returns>
        public static QueryClrCollectionType<TQueryType> CreateClrType<TQueryType>(TQueryType queryType, Type clrTypeDefinition) where TQueryType : QueryType
        {
            IQueryClrType queryClrType = queryType as IQueryClrType;
            ExceptionUtilities.CheckArgumentNotNull(clrTypeDefinition, "clrTypeDefinition");
            ExceptionUtilities.Assert(clrTypeDefinition.IsGenericTypeDefinition(), "clrTypeDefinition is not a generic type definition");
            ExceptionUtilities.CheckObjectNotNull(queryClrType, "queryType should implement IQueryClrType");
            Type clrTypeOfElement = clrTypeDefinition.MakeGenericType(queryClrType.ClrType);
            return new QueryClrCollectionType<TQueryType>(clrTypeOfElement, queryType, queryType.EvaluationStrategy);
        }

        /// <summary>
        /// Creates a QueryCollectionType which implements IQueryClrType so that codegen creates the right list type
        /// </summary>
        /// <typeparam name="TQueryType">The element type of the Collection</typeparam>
        /// <param name="queryType">The query type of the Collection</param>
        /// <param name="clrTypeDefinition">The generic type definition of the collection type, e.g.: List&lt;&gt; </param>
        /// <returns>A QueryClrCollectionType which is used as a hint by Code Generation</returns>
        public static QueryClrCollectionType<TQueryType> ToClrType<TQueryType>(QueryCollectionType queryType, Type clrTypeDefinition) where TQueryType : QueryType
        {
            ExceptionUtilities.CheckArgumentNotNull(clrTypeDefinition, "clrTypeDefinition");
            ExceptionUtilities.Assert(clrTypeDefinition.IsGenericTypeDefinition(), "clrTypeDefinition is not a generic type definition");
            TQueryType elementType = queryType.ElementType as TQueryType;
            return CreateClrType<TQueryType>(elementType, clrTypeDefinition);
        }
    }
}
