//---------------------------------------------------------------------
// <copyright file="DefaultEntitySetResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Implements the IEntitySetResolver interface.
    /// </summary>
    [ImplementationName(typeof(IEntitySetResolver), "Default")]
    public class DefaultEntitySetResolver : IEntitySetResolver
    {
        /// <summary>
        /// Gets or sets a descriptor for the OData service
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAstoriaServiceDescriptor ServiceDescriptor { get; set; }

        /// <summary>
        /// Resolves the base uri of the EntitySet
        /// </summary>
        /// <param name="entitySetName">The name of the entityset</param>
        /// <returns>The Uri to the entity set without the entity set name</returns>
        public Uri ResolveEntitySetBaseUri(string entitySetName)
        {
            return this.ServiceDescriptor.ServiceUri;
        }

        /// <summary>
        /// Resolves the uri of the EntitySet
        /// </summary>
        /// <param name="entitySetName">The name of the entityset</param>
        /// <returns>The Uri to the entity set </returns>
        public Uri ResolveEntitySetUri(string entitySetName)
        {
            return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.ServiceDescriptor.ServiceUri.OriginalString.TrimEnd('/'), entitySetName));
        }
    }
}
