//---------------------------------------------------------------------
// <copyright file="AstoriaQueryVerifierCapabilityInspector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Inspects the QueryVerifier to understand what level of capability it has
    /// </summary>
    [ImplementationName(typeof(IAstoriaQueryVerifierCapabilityInspector), "Default")]
    public class AstoriaQueryVerifierCapabilityInspector : IAstoriaQueryVerifierCapabilityInspector
    {
        /// <summary>
        /// Gets or sets the query verifier.
        /// </summary>
        /// <value>
        /// The query verifier.
        /// </value>
        [InjectDependency(IsRequired = true)]
        public IQueryVerifier QueryVerifier { get; set; }

        /// <summary>
        /// Indicates the QueryVerifier runs on the client
        /// </summary>
        /// <returns>True if executing on the client</returns>
        public virtual bool ExecutesOnDataServicesClient()
        {
            if (this.QueryVerifier is DataServiceExecuteVerifier || this.QueryVerifier is DataServiceQueryVerifier)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates that this query execution method supports projection when the keys aren't specified
        /// </summary>
        /// <returns>True if the executor works with projections without keys</returns>
        public virtual bool SupportsProjectionWithoutKeys()
        {
            return true;
        }
    }
}