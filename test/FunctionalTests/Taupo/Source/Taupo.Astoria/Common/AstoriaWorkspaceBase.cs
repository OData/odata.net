//---------------------------------------------------------------------
// <copyright file="AstoriaWorkspaceBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Workspace base class that has the properties necessary for QueryDataSetBuilder and QueryRepositoryBuilder
    /// </summary>
    public class AstoriaWorkspaceBase : Workspace
    {
        /// <summary>
        /// Gets or sets the query data set that is currently in use. Will be built by the first use of the specific query data set builder.
        /// </summary>
        internal IQueryDataSet CurrentQueryDataSet { get; set; }

        /// <summary>
        /// Gets or sets the query repository that is currently in use. Will be built by the first use of the specific query repository builder.
        /// </summary>
        internal QueryRepository CurrentQueryRepository { get; set; }
    }
}
