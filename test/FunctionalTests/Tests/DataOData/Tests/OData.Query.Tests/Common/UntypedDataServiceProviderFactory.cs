//---------------------------------------------------------------------
// <copyright file="UntypedDataServiceProviderFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.Common
{
    #region Namespaces
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.DataServiceProvider;

    #endregion Namespaces

    /// <summary>
    /// DataServiceProviderFactory to generate metadata from a model which is not backed by any CLR types.
    /// This factory doesn't support creation of query provider, only the metadata provider.
    /// </summary>
    [ImplementationName(typeof(IDataServiceProviderFactory), "Untyped non-query")]
    public class UntypedDataServiceProviderFactory : DataServiceProviderFactory
    {
        /// <summary>
        /// Generates a query resolver to resolve entity sets to IQueryable
        /// </summary>
        /// <param name="model">The conceptual schema for the workspace</param>
        /// <returns>a Query resolver to resolver entity sets to IQueryable</returns>
        public override IODataQueryProvider CreateQueryProvider(EntityModelSchema model)
        {
            throw new NotSupportedException("The UntypedDataServiceProviderFactory doesn't support creation of query provider.");
        }

        /// <summary>
        /// Gets the CLR instance type for the specified complex type.
        /// </summary>
        /// <param name="complex">The complex type to get the instance type for.</param>
        /// <param name="canReflectOnInstanceType">true if reflection over the instance type is allowed; otherwise false.</param>
        /// <returns>The CLR instance type to use.</returns>
        protected override Type GetComplexInstanceType(ComplexType complex, out bool canReflectOnInstanceType)
        {
            canReflectOnInstanceType = false;
            return typeof(DSPResource);
        }

        /// <summary>
        /// Gets the CLR instance type for the specified entity type.
        /// </summary>
        /// <param name="entity">The entity type to get the instance type for.</param>
        /// <param name="canReflectOnInstanceType">true if reflection over the instance type is allowed; otherwise false.</param>
        /// <returns>The CLR instance type to use.</returns>
        protected override Type GetEntityInstanceType(EntityType entity, out bool canReflectOnInstanceType)
        {
            canReflectOnInstanceType = false;
            return typeof(DSPResource);
        }
    }
}