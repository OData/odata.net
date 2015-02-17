//---------------------------------------------------------------------
// <copyright file="IODataConventionBasedLinkGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Component responsible for generating OData links based on conventions
    /// </summary>
    [ImplementationSelector("ODataConventionBasedLinkGenerator", DefaultImplementation = "Default", HelpText = "Responsible generating OData links based on well-established conventions")]
    public interface IODataConventionBasedLinkGenerator
    {
        /// <summary>
        /// Generates the conventional id for the given entity: http://serviceRoot/SetName(keyValues)
        /// </summary>
        /// <param name="entitySet">The entity set of the entity</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="keyValues">They values of the entity's key</param>
        /// <returns>The entity id</returns>
        string GenerateEntityId(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues);

        /// <summary>
        /// Generates the conventional edit link for the given entity: http://serviceRoot/SetName(keyValues)
        /// </summary>
        /// <param name="entitySet">The entity set of the entity</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="keyValues">They values of the entity's key</param>
        /// <returns>The convention-based edit link</returns>
        string GenerateEntityEditLink(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues);

        /// <summary>
        /// Generates the conventional stream edit link for the given entity: http://serviceRoot/SetName(keyValues)/$value
        /// </summary>
        /// <param name="entitySet">The entity set of the entity</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="keyValues">They values of the entity's key</param>
        /// <returns>The convention-based stream edit link</returns>
        string GenerateDefaultStreamEditLink(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues);

        /// <summary>
        /// Generates the conventional edit link for the given named stream: http://serviceRoot/SetName(keyValues)/StreamName
        /// </summary>
        /// <param name="entitySet">The entity set of the entity</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="keyValues">They values of the entity's key</param>
        /// <param name="streamName">The stream's name</param>
        /// <returns>The convention-based stream edit link</returns>
        string GenerateNamedStreamEditLink(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues, string streamName);
    }
}
