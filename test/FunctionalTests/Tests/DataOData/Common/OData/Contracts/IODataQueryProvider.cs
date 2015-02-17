//---------------------------------------------------------------------
// <copyright file="IODataQueryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    using System.Linq;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Provides a metadata and query source abstraction for a 
    /// data service's store.
    /// </summary>
    public interface IODataQueryProvider
    {
        /// <summary>Gets a value indicating whether null propagation is required in expression trees.</summary>
        bool IsNullPropagationRequired
        {
            get;
        }

        /// <summary>
        /// Returns the IQueryable that represents the entity set.
        /// </summary>
        /// <param name="entitySet">Entity set.</param>
        /// <returns>
        /// An IQueryable that represents the set; null if there is 
        /// no set for the specified name.
        /// </returns>
        IQueryable GetQueryRootForEntitySet(IEdmEntitySet entitySet);

        /// <summary>
        /// Invoke the given service operation and returns the results.
        /// </summary>
        /// <param name="serviceOperation">service operation to invoke.</param>
        /// <param name="parameters">value of parameters to pass to the service operation.</param>
        /// <returns>returns the result of the service operation. If the service operation returns void, then this should return null.</returns>
        object InvokeServiceOperation(IEdmOperationImport serviceOperation, object[] parameters);
    }
}
