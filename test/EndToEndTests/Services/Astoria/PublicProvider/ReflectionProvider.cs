//---------------------------------------------------------------------
// <copyright file="ReflectionProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider 
{
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using Microsoft.Test.OData.Services.AstoriaDefaultService;

    /// <summary>
    /// The reflection provider
    /// </summary>
    public class ReflectionProvider : ReflectionDataServiceProvider
    {
        /// <summary>
        /// Create an instance of class reflection provider
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="container">The reflection data source</param>
        public ReflectionProvider(ReflectionService service, DefaultContainer container)
            : base(new DataServiceProviderArgs(service, container, null, true)) {}

        /// <summary>
        /// Create an instance of class reflection provider
        /// </summary>
        /// <param name="dataServiceProviderArgs"></param>
        public ReflectionProvider(DataServiceProviderArgs dataServiceProviderArgs)
            : base(dataServiceProviderArgs) {}

        /// <summary>
        /// Override the GetQueryRootForResourceSet to fix the expression tree for Geo types and enum
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public override IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            return L2OParameterizedQueryProvider.CreateQuery(base.GetQueryRootForResourceSet(resourceSet));
        }
    }
}
