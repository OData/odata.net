//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
