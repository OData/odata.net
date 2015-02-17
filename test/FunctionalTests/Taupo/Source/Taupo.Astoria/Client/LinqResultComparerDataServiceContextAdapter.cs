//---------------------------------------------------------------------
// <copyright file="LinqResultComparerDataServiceContextAdapter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;

    /// <summary>
    /// Handles Context specific operations that are required for comparer to determine if values are equal
    ///  For DataServiceContext the only operation required is to determine what entities are tracked
    ///  by the context, all other methods are not required to do anything
    /// </summary>
    [ImplementationName(typeof(ILinqResultComparerContextAdapter), "DataContextComparerContextAdapter")]
    public class LinqResultComparerDataServiceContextAdapter : ILinqResultComparerContextAdapter
    {
        private List<object> currentEntities;

        /// <summary>
        /// Prepares the context for comparison.
        /// </summary>
        /// <param name="context">The context to prepare</param>
        public void PrepareContext(object context)
        {
        }

        /// <summary>
        /// On comparison of navigation properties, entities that were previously
        /// queried may be in the graph where null was expected
        /// </summary>
        /// <param name="value">value to be checked for tracking</param>
        /// <returns>True if the value is tracked</returns>
        public bool IsObjectTrackedByContext(object value)
        {
            return this.currentEntities.Contains(value);
        }

        /// <summary>
        /// Method is called directly after a query is executed
        /// useful for getting a list of entities tracked in the context
        /// </summary>
        /// <param name="context">Context that is used for the query execute</param>
        public void AfterQueryExecuted(object context)
        {
            var dataServiceContext = this.GetDataServiceContext(context);
            this.currentEntities = dataServiceContext.Entities.Select(c => c.Entity).ToList();
        }

        /// <summary>
        /// Restore the context to the state before the comparison (basically reverts the PrepareContext method)
        /// </summary>
        /// <param name="context">The context to restore</param>
        public void RestoreContext(object context)
        {
        }

        /// <summary>
        /// Get the object context from the strongly typed context (typically from generated code)
        /// </summary>
        /// <param name="stronglyTypedContext">The strongly typed context</param>
        /// <returns>The object context for the strongly typed context</returns>
        protected virtual DataServiceContext GetDataServiceContext(object stronglyTypedContext)
        {
            var objectContext = stronglyTypedContext as DataServiceContext;
            ExceptionUtilities.Assert(objectContext != null, "Strongly typed context is not of Type DataServiceContext: {0}", stronglyTypedContext);

            return objectContext;
        }
    }
}
