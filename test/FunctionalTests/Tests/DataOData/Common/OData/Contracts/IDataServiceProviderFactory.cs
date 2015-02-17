//---------------------------------------------------------------------
// <copyright file="IDataServiceProviderFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    #endregion Namespaces

    /// <summary>
    /// A factory interface for creating Metadata and query provider for use by verifiers and translators
    /// </summary>
    [ImplementationSelector("DataServiceMetadataProviderFactory", DefaultImplementation = "Default", HelpText = "The factory to use to create an instance of IDSMP from a particular model")]
    public interface IDataServiceProviderFactory
    {
        /// <summary>
        /// Generates an EDM model from an entity model schema
        /// </summary>
        /// <returns>An EDM model</returns>
        IEdmModel CreateMetadataProvider(EntityModelSchema model);

        /// <summary>
        /// Generates a query resolver to resolve entity sets to IQueryable
        /// </summary>
        /// <param name="model">The conceptual schema for the workspace</param>
        /// <returns>a Query resolver to resolver entity sets to IQueryable</returns>
        IODataQueryProvider CreateQueryProvider(EntityModelSchema model);
    }
}
