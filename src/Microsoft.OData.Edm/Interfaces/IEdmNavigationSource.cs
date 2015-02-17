//---------------------------------------------------------------------
// <copyright file="IEdmNavigationSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines the kind of navigation source
    /// </summary>
    public enum EdmNavigationSourceKind
    {
        /// <summary>
        /// Represents a value with an unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents EntitySet
        /// </summary>
        EntitySet,

        /// <summary>
        /// Represents Singleton
        /// </summary>
        Singleton,

        /// <summary>
        /// Represents ContainedEntitySet
        /// </summary>
        ContainedEntitySet,

        /// <summary>
        /// Represents UnknownEntitySet
        /// </summary>
        UnknownEntitySet,
    }

    /// <summary>
    /// Represents an EDM navigation source.
    /// </summary>
    public interface IEdmNavigationSource : IEdmNamedElement
    {
        /// <summary>
        /// Gets the navigation property bindings of this navigation source.
        /// </summary>
        IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings { get; }

        /// <summary>
        /// Gets the path of this navigation source.
        /// </summary>
        IEdmPathExpression Path { get; }

        /// <summary>
        /// Gets the type of this navigation source.
        /// </summary>
        IEdmType Type { get; }

        /// <summary>
        /// Finds the navigation source that a navigation property targets.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <returns>The navigation source that the navigation propertion targets</returns>
        IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty);
    }
}
