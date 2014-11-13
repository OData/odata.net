//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
