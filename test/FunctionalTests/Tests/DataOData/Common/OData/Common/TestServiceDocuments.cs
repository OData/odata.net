//---------------------------------------------------------------------
// <copyright file="TestServiceDocuments.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;

    #endregion Namespaces

    /// <summary>
    /// Helper class to create all interesting service document payloads.
    /// </summary>
    public static class TestServiceDocuments
    {
        /// <summary>
        /// Creates a set of interesting service documents.
        /// </summary>
        /// <param name="withTitles">true if workspaces and collections should have a title; otherwise false.</param>
        /// <returns>List of interesting service documents.</returns>
        public static IEnumerable<ServiceDocumentInstance> CreateServiceDocuments(bool withTitles, string baseUri)
        {
            // empty
            yield return PayloadBuilder.ServiceDocument().Workspace(
                PayloadBuilder.Workspace().WithTitle(withTitles ? "WorkspaceTitle" : null));

            // one entity set
            yield return PayloadBuilder.ServiceDocument().Workspace(
                PayloadBuilder.Workspace()
                    .WithTitle(withTitles ? "WorkspaceTitle" : null)
                    .ResourceCollection(withTitles ? "FirstCollectionTitle" : null, baseUri + "FirstCollection"));

            // three entity sets
            yield return PayloadBuilder.ServiceDocument().Workspace(
                PayloadBuilder.Workspace()
                    .WithTitle(withTitles ? "WorkspaceTitle" : null)
                    .ResourceCollection(withTitles ? "FirstCollectionTitle" : null, baseUri + "FirstCollection")
                    .ResourceCollection(withTitles ? "SecondCollectionTitle" : null, baseUri + "SecondCollection")
                    .ResourceCollection(withTitles ? "ThirdCollectionTitle" : null, baseUri + "ThirdCollection"));
        }

    }
}
