//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents an EDM entity set.
    /// </summary>
    public interface IEdmEntitySet : IEdmEntityContainerElement
    {
        /// <summary>
        /// Gets the entity type contained in this entity set.
        /// </summary>
        IEdmEntityType ElementType { get; }

        /// <summary>
        /// Gets the navigation targets of this entity set.
        /// </summary>
        IEnumerable<IEdmNavigationTargetMapping> NavigationTargets { get; }

        /// <summary>
        /// Finds the entity set that a navigation property targets.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// /// <returns>The entity set that the navigation propertion targets, or null if no such entity set exists.</returns>
        IEdmEntitySet FindNavigationTarget(IEdmNavigationProperty navigationProperty);
    }
}
