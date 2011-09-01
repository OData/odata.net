//   Copyright 2011 Microsoft Corporation
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
using System.Linq;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM structured value.
    /// </summary>
    public abstract class EdmStructuredValue : EdmValue, IEdmStructuredValue
    {
        private readonly IEnumerable<IEdmPropertyValue> propertyValues;

        /// <summary>
        /// Initializes a new instance of the EdmStructuredValue class.
        /// </summary>
        /// <param name="type">Type that describes this value.</param>
        /// <param name="propertyValues">Child values of this value.</param>
        protected EdmStructuredValue(IEdmStructuredTypeReference type, IEnumerable<IEdmPropertyValue> propertyValues)
            : base(type)
        {
            this.propertyValues = propertyValues;
        }

        /// <summary>
        /// Gets the property values of this structured value.
        /// </summary>
        public IEnumerable<IEdmPropertyValue> PropertyValues
        {
            get { return this.propertyValues; }
        }

        /// <summary>
        /// Retrieves the value corresponding to the given property. Returns null if no such value exists.
        /// </summary>
        /// <param name="property">The property that describes the value being found.</param>
        /// <returns>The requested value, or null if no such value exists.</returns>
        public IEdmPropertyValue FindPropertyValue(IEdmProperty property)
        {
            return this.propertyValues.SingleOrDefault(p => p.Property == property);
        }
    }
}
