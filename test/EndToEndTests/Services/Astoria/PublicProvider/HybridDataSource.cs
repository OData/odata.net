//---------------------------------------------------------------------
// <copyright file="HybridDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider 
{
    using Microsoft.Test.OData.Services.Astoria;
    using Microsoft.Test.OData.Services.AstoriaDefaultService;

    /// <summary>
    /// Hybrid service data source
    /// </summary>
    public class HybridDataSource
    {
        /// <summary>
        /// The database source
        /// </summary>
        public AstoriaDefaultServiceDBEntities DatabaseSource { get; set; }

        /// <summary>
        /// The reflection source
        /// </summary>
        public DefaultContainer ReflectionDataSource { get; set; }
    }
}
