//---------------------------------------------------------------------
// <copyright file="IEntitySetResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Interface allows developers to resolve the Uri to access an entityset given its name
    /// </summary>
    [ImplementationSelector("EntitySetResolver")]
    public interface IEntitySetResolver
    {
        /// <summary>
        /// Resolves the base uri of the EntitySet
        /// </summary>
        /// <param name="entitySetName">The name of the entityset</param>
        /// <returns>The Uri to the entity set without the entity set name</returns>
        Uri ResolveEntitySetBaseUri(string entitySetName);

        /// <summary>
        /// Resolves the uri of the EntitySet
        /// </summary>
        /// <param name="entitySetName">The name of the entityset</param>
        /// <returns>The Uri to the entity set </returns>
        Uri ResolveEntitySetUri(string entitySetName);
    }
}
