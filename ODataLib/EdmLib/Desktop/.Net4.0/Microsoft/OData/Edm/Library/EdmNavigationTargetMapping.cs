//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a mapping from an EDM navigation property to an entity set.
    /// </summary>
    public class EdmNavigationTargetMapping : IEdmNavigationTargetMapping
    {
        private IEdmNavigationProperty navigationProperty;
        private IEdmEntitySet targetEntitySet;

        /// <summary>
        /// Creates a new navigation target mapping.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <param name="targetEntitySet">The entity set that the navigation propertion targets.</param>
        public EdmNavigationTargetMapping(IEdmNavigationProperty navigationProperty, IEdmEntitySet targetEntitySet)
        {
            this.navigationProperty = navigationProperty;
            this.targetEntitySet = targetEntitySet;
        }

        /// <summary>
        /// Gets the navigation property.
        /// </summary>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Gets the target entity set.
        /// </summary>
        public IEdmEntitySet TargetEntitySet
        {
            get { return this.targetEntitySet; }
        }
    }
}
