//---------------------------------------------------------------------
// <copyright file="IAstoriaQueryRepositoryHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Taupo.Common;

    /// <summary>
    /// Helper methods for Astoria tests to limit root queries
    /// </summary>
    [ImplementationSelector("AstoriaQueryRepositoryHelper", DefaultImplementation = "Default")]
    public interface IAstoriaQueryRepositoryHelper
    {
        /// <summary>
        /// Performs the specified action on all root queries within the repository
        /// </summary>
        /// <param name="action">the action to perform</param>
        /// <remarks>
        /// For whatever reason, the CLR on the Phone (.netCF) does not allow static methods in a static class to call each other.
        /// Doing so will result in a runtime TypeLoadException which is quite painful to debug.
        /// The below code, while it looks sub-optimal is the only way to get this type to load on the Windows Phone platform
        /// </remarks>
        void RunOnAllRootQueries(Action<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>> action);

        /// <summary>
        /// Performs the specified action on all EntitySet based root queries
        /// </summary>
        /// <param name="action">the action to perform</param>
        /// <remarks>
        /// For whatever reason, the CLR on the Phone (.netCF) does not allow static methods in a static class to call each other.
        /// Doing so will result in a runtime TypeLoadException which is quite painful to debug.
        /// The below code, while it looks sub-optimal is the only way to get this type to load on the Windows Phone platform
        /// </remarks>
        void RunOnEntitySetRootQueries(Action<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>> action);

        /// <summary>
        /// Performs the specified action on all EntitySet based root queries
        /// </summary>
        /// <param name="queryFilter">Filter query</param>
        /// <param name="action">the action to perform</param>
        /// <remarks>
        /// For whatever reason, the CLR on the Phone (.netCF) does not allow static methods in a static class to call each other.
        /// Doing so will result in a runtime TypeLoadException which is quite painful to debug.
        /// The below code, while it looks sub-optimal is the only way to get this type to load on the Windows Phone platform
        /// </remarks>
        void RunOnEntitySetRootQueries(
            Func<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>, bool> queryFilter,
            Action<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>> action);

        /// <summary>
        /// Performs the specified action on a subset of root queries within the repository
        /// </summary>
        /// <param name="queryFilter">the filter used to generate the subset</param>
        /// <param name="action">the action to perform</param>
        /// <remarks>
        /// For whatever reason, the CLR on the Phone (.netCF) does not allow static methods in a static class to call each other.
        /// Doing so will result in a runtime TypeLoadException which is quite painful to debug.
        /// The below code, while it looks sub-optimal is the only way to get this type to load on the Windows Phone platform
        /// </remarks>
        void RunOnRootQueries(
            Func<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>, bool> queryFilter,
            Action<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>> action);
    }
}