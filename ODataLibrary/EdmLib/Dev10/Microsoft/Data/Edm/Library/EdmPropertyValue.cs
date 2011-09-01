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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a value of an EDM property.
    /// </summary>
    public class EdmPropertyValue : IEdmPropertyValue
    {
        private readonly IEdmProperty property;
        private readonly IEdmValue value;

        /// <summary>
        /// Initializes a new instance of the EdmPropertyValue class.
        /// </summary>
        /// <param name="property">Property describing the this value.</param>
        /// <param name="value">Value represented by this value.</param>
        public EdmPropertyValue(IEdmProperty property, IEdmValue value)
        {
            this.property = property;
            this.value = value;
        }

        /// <summary>
        /// Gets the property that corresponds to this property value.
        /// </summary>
        public IEdmProperty Property
        {
            get { return this.property; }
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public IEdmValue Value
        {
            get { return this.value; }
        }
    }
}
