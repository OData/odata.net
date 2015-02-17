//---------------------------------------------------------------------
// <copyright file="EntityFrameworkQueryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.OData.Contracts;

    /// <summary>
    /// This class provides root set resolution support for an EF backed provider
    /// </summary>
    public class EntityFrameworkQueryProvider : IODataQueryProvider
    {
        private DataContext contextInstance;

        /// <summary>
        /// Creates or Initializes an instance of the EntityFrameworkQueryProvider Type
        /// </summary>
        /// <param name="context">The objectcontext instance to get the root queries from</param>
        public EntityFrameworkQueryProvider(DataContext context)
        {
            this.contextInstance = context;
        }

        /// <summary>
        /// Gets a value indicating whether null propagation is requried in creating queries
        /// </summary>
        public bool IsNullPropagationRequired
        {
            get { return false; }
        }

        /// <summary>
        /// A helper property for serialization of results, represents the root set of the current running query
        /// </summary>
        public string RootSetName { get; private set; }

        /// <summary>
        /// Returns the IQueryable that represents the entity set.
        /// </summary>
        /// <param name="entitySet">Entity set representing the entity set.</param>
        /// <returns>
        /// An IQueryable that represents the set; null if there is 
        /// no set for the specified name.
        /// </returns>
        public IQueryable GetQueryRootForEntitySet(IEdmEntitySet entitySet)
        {
            this.RootSetName = entitySet.Name;
            return this.contextInstance.GetRootQuery(entitySet);
        }

        /// <summary>
        /// Invoke the given service operation and returns the results.
        /// </summary>
        /// <param name="serviceOperation">service operation to invoke.</param>
        /// <param name="parameters">value of parameters to pass to the service operation.</param>
        /// <returns>returns the result of the service operation. If the service operation returns void, then this should return null.</returns>
        public object InvokeServiceOperation(IEdmOperationImport serviceOperation, object[] parameters)
        {
            throw new NotSupportedException();
        }
    }
}