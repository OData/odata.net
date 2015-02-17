//---------------------------------------------------------------------
// <copyright file="ServiceDocumentBasedEntitySetResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Implements the IEntitySetResolver interface.
    /// </summary>
    [ImplementationName(typeof(IEntitySetResolver), "ServiceDocBased")]
    public class ServiceDocumentBasedEntitySetResolver : IEntitySetResolver
    {
        /// <summary>
        /// A dictionary of entityset name to the entityset uri in the data service
        /// </summary>
        private IDictionary<string, Uri> entitySetLinks;

        /// <summary>
        /// Initializes a new instance of the ServiceDocumentBasedEntitySetResolver class
        /// </summary>
        /// <param name="entitySetLinks">A dictionary of entityset name to the entityset uri in the data service</param>
        public ServiceDocumentBasedEntitySetResolver(IDictionary<string, Uri> entitySetLinks)
        {
            ExceptionUtilities.CheckObjectNotNull(entitySetLinks, "EntitySetLinks was not set");
            this.entitySetLinks = entitySetLinks;
        }

        /// <summary>
        /// Resolves the base uri of the EntitySet
        /// </summary>
        /// <param name="entitySetName">The name of the entityset</param>
        /// <returns>The Uri to the entity set without the entity set name</returns>
        public Uri ResolveEntitySetBaseUri(string entitySetName)
        {
            ExceptionUtilities.CheckObjectNotNull(this.entitySetLinks, "EntitySetLinks was not set");
            Uri entitySetUri = this.ResolveEntitySetUri(entitySetName);
            return new Uri(entitySetUri.OriginalString.Replace(string.Format(CultureInfo.InvariantCulture, "/{0}", entitySetName), "/"));
        }

        /// <summary>
        /// Resolves the uri of the EntitySet
        /// </summary>
        /// <param name="entitySetName">The name of the entityset</param>
        /// <returns>The Uri to the entity set </returns>
        public Uri ResolveEntitySetUri(string entitySetName)
        {
            ExceptionUtilities.CheckObjectNotNull(this.entitySetLinks, "EntitySetLinks was not set");
            ExceptionUtilities.Assert(this.entitySetLinks.ContainsKey(entitySetName), "The Service document didn't contain a collection uri for the entity set '{0}'", entitySetName);
            return this.entitySetLinks[entitySetName];
        }
    }
}
