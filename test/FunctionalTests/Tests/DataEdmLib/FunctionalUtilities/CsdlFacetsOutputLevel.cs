//---------------------------------------------------------------------
// <copyright file="CsdlFacetsOutputLevel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    public enum CsdlFacetsOutputLevel
    {
        /// <summary>
        /// Not specifically specified - implementation can decide what facets to output
        /// </summary>
        NotSpecified,

        /// <summary>
        /// Suppress all facets (do not output)
        /// </summary>
        SuppressAll,

        /// <summary>
        /// Suppress only if value is the same as default
        /// </summary>
        SuppressIfSameAsDefault,

        /// <summary>
        /// Verbose - always output facets (never suppress)
        /// </summary>
        Verbose,
    }
}
