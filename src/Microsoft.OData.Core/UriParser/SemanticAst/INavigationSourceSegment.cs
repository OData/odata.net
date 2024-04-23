//---------------------------------------------------------------------
// <copyright file="INavigationSourceSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Represents a navigation source segment.
    /// </summary>
    public interface INavigationSourceSegment
    {
        /// <summary>The navigation source targeted by this segment.</summary>
        IEdmNavigationSource NavigationSource { get; }
    }
}
