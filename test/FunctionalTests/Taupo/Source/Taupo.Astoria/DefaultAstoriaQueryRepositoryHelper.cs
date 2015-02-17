//---------------------------------------------------------------------
// <copyright file="DefaultAstoriaQueryRepositoryHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Helper methods for Astoria tests
    /// </summary>
    [ImplementationName(typeof(IAstoriaQueryRepositoryHelper), "Default")]
    public class DefaultAstoriaQueryRepositoryHelper : IAstoriaQueryRepositoryHelper
    {
        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        /// <value>
        /// The repository.
        /// </value>
        [InjectDependency(IsRequired = true)]
        public QueryRepository Repository { get; set; }

        /// <summary>
        /// Performs the specified action on all root queries within the repository
        /// </summary>
        /// <param name="action">the action to perform</param>
        /// <remarks>
        /// For whatever reason, the CLR on the Phone (.netCF) does not allow static methods in a static class to call each other.
        /// Doing so will result in a runtime TypeLoadException which is quite painful to debug.
        /// The below code, while it looks sub-optimal is the only way to get this type to load on the Windows Phone platform
        /// </remarks>
        public void RunOnAllRootQueries(Action<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>> action)
        {
            ExceptionUtilities.CheckArgumentNotNull(action, "action");

            var rootQueries = this.Repository.RootQueries.Collections<QueryStructuralType>();
            ExceptionUtilities.CheckCollectionNotEmpty(rootQueries, "rootQueries");

            foreach (var rootQuery in rootQueries)
            {
                action(rootQuery);
            }
        }

        /// <summary>
        /// Performs the specified action on all EntitySet based root queries
        /// </summary>
        /// <param name="action">the action to perform</param>
        /// <remarks>
        /// For whatever reason, the CLR on the Phone (.netCF) does not allow static methods in a static class to call each other.
        /// Doing so will result in a runtime TypeLoadException which is quite painful to debug.
        /// The below code, while it looks sub-optimal is the only way to get this type to load on the Windows Phone platform
        /// </remarks>
        public void RunOnEntitySetRootQueries(Action<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>> action)
        {
            ExceptionUtilities.CheckArgumentNotNull(action, "action");

            var rootQueries = this.Repository.RootQueries.Collections<QueryStructuralType>();
            ExceptionUtilities.CheckCollectionNotEmpty(rootQueries, "rootQueries");

            foreach (var rootQuery in rootQueries)
            {
                if (rootQuery.Expression.IsRootEntitySetQuery())
                {
                    action(rootQuery);
                }
            }
        }

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
        public void RunOnEntitySetRootQueries(
            Func<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>, bool> queryFilter,
            Action<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>> action)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryFilter, "queryFilter");
            ExceptionUtilities.CheckArgumentNotNull(action, "action");

            var rootQueries = this.Repository.RootQueries.Collections<QueryStructuralType>();
            ExceptionUtilities.CheckCollectionNotEmpty(rootQueries, "rootQueries");

            bool wasQueryRun = false;
            foreach (var rootQuery in rootQueries)
            {
                if (rootQuery.Expression.IsRootEntitySetQuery())
                {
                    if (queryFilter(rootQuery))
                    {
                        wasQueryRun = true;
                        action(rootQuery);
                    }
                }
            }

            if (!wasQueryRun)
            {
                throw new TestSkippedException("No root queries were found that matched the given filter.");
            }
        }

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
        public void RunOnRootQueries(
            Func<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>, bool> queryFilter,
            Action<TypedQueryExpression<QueryCollectionType<QueryStructuralType>>> action)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryFilter, "queryFilter");
            ExceptionUtilities.CheckArgumentNotNull(action, "action");

            var rootQueries = this.Repository.RootQueries.Collections<QueryStructuralType>();
            ExceptionUtilities.CheckCollectionNotEmpty(rootQueries, "rootQueries");

            bool wasQueryRun = false;
            foreach (var rootQuery in rootQueries)
            {
                if (queryFilter(rootQuery))
                {
                    wasQueryRun = true;                
                    action(rootQuery);
                }
            }

            if (!wasQueryRun)
            {
                throw new TestSkippedException("No root queries were found that matched the given filter.");
            }
        }
    }
}