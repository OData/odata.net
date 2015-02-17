//---------------------------------------------------------------------
// <copyright file="ODataObjectResultComparerContextAdapter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;

    /// <summary>
    /// ODataLib specific implementation that does nothing as 
    /// there is no Context that actively tracks objects
    /// </summary>
    [ImplementationName(typeof(ILinqResultComparerContextAdapter), "ODataLib")]
    public class ODataObjectResultComparerContextAdapter : ILinqResultComparerContextAdapter
    {
        /// <summary>
        /// Method is called directly after a query is executed
        /// useful for getting a list of entities tracked in the context
        /// </summary>
        /// <param name="context">Context that is used for the query execute</param>
        public void AfterQueryExecuted(object context)
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
            return false;
        }

        /// <summary>
        /// Prepares the context for comparison.
        /// </summary>
        /// <param name="context">The context to prepare</param>
        public void PrepareContext(object context)
        {
        }

        /// <summary>
        /// Restore the context to the state before the comparison (basically reverts the PrepareContext method)
        /// </summary>
        /// <param name="context">The context to restore</param>
        public void RestoreContext(object context)
        {
        }
    }
}
