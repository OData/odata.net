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

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents an EDM structured value.
    /// </summary>
    public interface IEdmStructuredValue : IEdmValue
    {
        /// <summary>
        /// Gets the property values of this structured value.
        /// </summary>
        IEnumerable<IEdmPropertyValue> PropertyValues { get; }

        /// <summary>
        /// Finds the value corresponding to the provided IEdmProperty.
        /// </summary>
        /// <param name="property">Property to find the value of.</param>
        /// <returns>The found property, or null if no property was found.</returns>
        IEdmPropertyValue FindPropertyValue(IEdmProperty property);
    }
}
