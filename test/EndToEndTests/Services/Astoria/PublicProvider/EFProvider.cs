//---------------------------------------------------------------------
// <copyright file="EFProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider 
{
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using Microsoft.Test.OData.Services.Astoria;

    /// <summary>
    /// The EF Provider which inherits from EntityFrameworkDataServiceProvider
    /// </summary>
    public class EFProvider : EntityFrameworkDataServiceProvider
    {
        /// <summary>
        /// Create an instance of type EFProvider
        /// </summary>
        /// <param name="service"></param>
        /// <param name="container"></param>
        public EFProvider(EFService service, AstoriaDefaultServiceDBEntities container)
            : base(new DataServiceProviderArgs(service, container, null, false)) {}

        /// <summary>
        /// Create an instance of type EFProvider
        /// </summary>
        /// <param name="dataServiceProviderArgs"></param>
        internal EFProvider(DataServiceProviderArgs dataServiceProviderArgs)
            : base(dataServiceProviderArgs) {}

        /// <summary>
        /// Override the query root
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public override IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            // First parameterize the expression tree, then fix the tree for Geo and Enum
            return EFParameterizedQueryProvider.CreateQuery(base.GetQueryRootForResourceSet(resourceSet));
        }

        /// <summary>
        /// Override the get resource to get underlying ObjectQuery for base.GetResource
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fullTypeName"></param>
        /// <returns></returns>
        public override object GetResource(IQueryable query, string fullTypeName)
        {
            var objectQueryWrapper = query as IObjectQueryWrapper;
            var objectQuery = objectQueryWrapper == null ? query : objectQueryWrapper.ObjectQuery;
            return base.GetResource(objectQuery, fullTypeName);
        }
    }
}
