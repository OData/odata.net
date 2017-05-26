//---------------------------------------------------------------------
// <copyright file="DataServiceProviderFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.Common
{
    #region Namespaces
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Base class for a factory implementation for creating Metadata and query provider for use by verifiers and translators.
    /// Implements the IDSMP creation from EntityModelSchema but doesn't create the query provider.
    /// </summary>
    public abstract class DataServiceProviderFactory : IDataServiceProviderFactory
    {
        /// <summary>
        /// Gets or sets a value indicating whether the expression translator injects null check nodes in the tree
        /// </summary>
        [InjectTestParameter("NullPropagationRequired", DefaultValueDescription = "true")]
        public bool NullPropagationRequired { get; set; }

        /// <summary>
        /// Converter from an entity model to schema to an EDM model.
        /// </summary>
        [InjectDependency(IsRequired=true)]
        public ODataQueryEntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

        /// <summary>
        /// Generate an EDM model representation of <paramref name="schema"/>
        /// </summary>
        /// <param name="schema">The entity model schema to generate the EDM model from.</param>
        /// <returns>The generated EDM model.</returns>
        public virtual IEdmModel CreateMetadataProvider(EntityModelSchema schema)
        {
            IEdmModel model = this.EntityModelSchemaToEdmModelConverter.Convert(schema);
            return model;
        }

        /// <summary>
        /// Generates a query resolver to resolve entity sets to IQueryable
        /// </summary>
        /// <param name="model">The conceptual schema for the workspace</param>
        /// <returns>a Query resolver to resolver enity sets to IQueryable</returns>
        public virtual IODataQueryProvider CreateQueryProvider(EntityModelSchema model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the CLR instance type for the specified complex type.
        /// </summary>
        /// <param name="complex">The complex type to get the instance type for.</param>
        /// <param name="canReflectOnInstanceType">true if reflection over the instance type is allowed; otherwise false.</param>
        /// <returns>The CLR instance type to use.</returns>
        protected abstract Type GetComplexInstanceType(ComplexType complex, out bool canReflectOnInstanceType);

        /// <summary>
        /// Gets the CLR instance type for the specified entity type.
        /// </summary>
        /// <param name="entity">The entity type to get the instance type for.</param>
        /// <param name="canReflectOnInstanceType">true if reflection over the instance type is allowed; otherwise false.</param>
        /// <returns>The CLR instance type to use.</returns>
        protected abstract Type GetEntityInstanceType(EntityType entity, out bool canReflectOnInstanceType);
    }
}
