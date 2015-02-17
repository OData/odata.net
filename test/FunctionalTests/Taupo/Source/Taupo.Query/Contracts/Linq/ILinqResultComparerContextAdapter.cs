//---------------------------------------------------------------------
// <copyright file="ILinqResultComparerContextAdapter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Handles Context specific operations that are required for comparer to determine if values are equal
    /// This interface must be called in the following order by the LinqResultComparerBase
    ///   1) PrepareContext once 
    ///   2) BeforeQueryExecuted once
    ///   3) AfterQueryExecuted once
    ///   4) IsObjectTrackedByContext called zero to many times
    ///   5) RestoreContext once
    /// It is allowed to call an instance again but they must be called in this order and amount of times
    /// </summary>
    [ImplementationSelector("LinqResultComparerContextAdapter", HelpText = "Adapter used to determine how to compare navigation properties when objects are tracked")]
    public interface ILinqResultComparerContextAdapter
    {
        /// <summary>
        /// Prepares the context for comparison.
        /// </summary>
        /// <param name="context">The context to prepare</param>
        void PrepareContext(object context);

        /// <summary>
        /// On comparison of navigation properties, entities that were previously
        /// queried may be in the graph where null was expected
        /// </summary>
        /// <param name="value">value to be checked for tracking</param>
        /// <returns>True if the value is tracked</returns>
        bool IsObjectTrackedByContext(object value);

        /// <summary>
        /// Method is called directly after a query is executed
        /// useful for getting a list of entities tracked in the context
        /// </summary>
        /// <param name="context">Context that is used for the query execute</param>
        void AfterQueryExecuted(object context);

        /// <summary>
        /// Restore the context to the state before the comparison (basically reverts the PrepareContext method)
        /// </summary>
        /// <param name="context">The context to restore</param>
        void RestoreContext(object context);
    }
}
