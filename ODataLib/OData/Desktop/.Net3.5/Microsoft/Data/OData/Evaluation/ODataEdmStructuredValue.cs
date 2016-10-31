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

namespace Microsoft.Data.OData.Evaluation
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library.Values;
    using Microsoft.Data.Edm.Values;
    using Microsoft.Data.OData.Metadata;
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
