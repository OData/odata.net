//---------------------------------------------------------------------
// <copyright file="IEdmContainedEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM contained entity set.
    /// </summary>
    public interface IEdmContainedEntitySet : IEdmEntitySetBase
    {
        /// <summary>The parent navigation source of this contained entity set.</summary>
        IEdmNavigationSource ParentNavigationSource { get; }

        /// <summary>The navigation property of this contained entity set.</summary>
        IEdmNavigationProperty NavigationProperty { get; }
    }
}
