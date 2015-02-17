//---------------------------------------------------------------------
// <copyright file="ODataTestWorkspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData
{
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// ODataServiceDocument which groups together interesting properties for ODataTests
    /// </summary>
    public class ODataTestWorkspace : AstoriaWorkspaceBase
    {
        /// <summary>
        /// Gets or sets the assembly containing the clr types for entity types
        /// </summary>
        public Assembly ObjectLayerAssembly { get; set; }

        /// <summary>
        /// Gets or sets ResourceLookup for resources in Microsoft.OData.Core
        /// </summary>
        /// <value>Resource lookup</value>
        public IStringResourceVerifier SystemDataODataStringVerifier { get; set; }
    }
}
