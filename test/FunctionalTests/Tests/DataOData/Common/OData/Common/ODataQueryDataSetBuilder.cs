//---------------------------------------------------------------------
// <copyright file="ODataQueryDataSetBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Query;

    /// <summary>
    /// Query data-set builder for OData library testing that is seperate from the one used for data-services (astoria) testing
    /// Note: this is primarily to avoid introducing a dependency on IAstoriaServiceDescriptor which AstoriaQueryDataSetBuilder indirectly depends on
    /// </summary>
    public class ODataQueryDataSetBuilder : AstoriaQueryDataSetBuilderBase
    {
    }
}