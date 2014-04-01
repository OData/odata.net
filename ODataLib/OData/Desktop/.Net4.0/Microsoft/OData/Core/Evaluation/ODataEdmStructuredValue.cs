//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Evaluation
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmValue"/> implementation of an OData entry or complex value.
    /// </summary>
    internal sealed class ODataEdmStructuredValue : EdmValue, IEdmStructuredValue
    {
        /// <summary>Properties of an OData entry or complex value.</summary>
        private readonly IEnumerable<ODataProperty> properties;

        /// <summary>The type of this structured value.</summary>
        private readonly IEdmStructuredTypeReference structuredType;

        /// <summary>
        /// Creates a new Edm structured value from an OData entry.
        /// </summary>
        /// <param name="entry">The <see cref="ODataEntry"/> to create the structured value for.</param>
        internal ODataEdmStructuredValue(ODataEntry entry)
            : base(entry.GetEdmType())
        {
            Debug.Assert(entry != null, "entry != null");

            this.properties = entry.NonComputedProperties;
            this.structuredType = this.Type == null ? null : this.Type.AsStructured();
        }

        /// <summary>
        /// Creates a new Edm structured value from an OData complex value.
        /// </summary>
        /// <param name="complexValue">The <see cref="ODataComplexValue"/> to create the structured value for.</param>
        internal ODataEdmStructuredValue(ODataComplexValue complexValue)
            : base(complexValue.GetEdmType())
        {
            Debug.Assert(complexValue != null, "complexValue != null");

            this.properties = complexValue.Properties;
            this.structuredType = this.Type == null ? null : this.Type.AsStructured();
        }

        /// <summary>
        /// Gets the property values of this structured value.
        /// </summary>
        public IEnumerable<IEdmPropertyValue> PropertyValues
        {
            get 
            {
                if (this.properties != null)
                {
                    foreach (ODataProperty property in this.properties)
                    {
                        yield return property.GetEdmPropertyValue(this.structuredType);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get
            {
                return EdmValueKind.Structured;
            }
        }

        /// <summary>
        /// Finds the value corresponding to the provided property name.
        /// </summary>
        /// <param name="propertyName">Property to find the value of.</param>
        /// <returns>The found property, or null if no property was found.</returns>
        public IEdmPropertyValue FindPropertyValue(string propertyName)
        {
            ODataProperty property = this.properties == null ? null : this.properties.Where(p => p.Name == propertyName).FirstOrDefault();
            if (property == null)
            {
                return null;
            }

            return property.GetEdmPropertyValue(this.structuredType);
        }
    }
}
