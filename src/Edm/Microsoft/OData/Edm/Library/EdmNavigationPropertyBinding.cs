//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
