//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a mapping from an EDM navigation property to a navigation source.
    /// </summary>
    public class EdmNavigationPropertyBinding : IEdmNavigationPropertyBinding
    {
        private IEdmNavigationProperty navigationProperty;
        private IEdmNavigationSource target;

        /// <summary>
        /// Creates a new navigation property binding.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <param name="target">The navigation source that the navigation property targets.</param>
        public EdmNavigationPropertyBinding(IEdmNavigationProperty navigationProperty, IEdmNavigationSource target)
        {
            this.navigationProperty = navigationProperty;
            this.target = target;
        }

        /// <summary>
        /// Gets the navigation property.
        /// </summary>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Gets the target navigation source.
        /// </summary>
        public IEdmNavigationSource Target
        {
            get { return this.target; }
        }
    }
}
