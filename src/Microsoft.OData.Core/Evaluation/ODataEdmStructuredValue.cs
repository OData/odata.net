//---------------------------------------------------------------------
// <copyright file="ODataEdmStructuredValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Metadata;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Evaluation
{
    /// <summary>
    /// An <see cref="IEdmValue"/> implementation of an OData resource or complex value.
    /// </summary>
    internal sealed class ODataEdmStructuredValue : EdmValue, IEdmStructuredValue
    {
        /// <summary>Properties of an OData resource or complex value.</summary>
        private readonly IEnumerable<ODataProperty> properties;

        /// <summary>The type of this structured value.</summary>
        private readonly IEdmStructuredTypeReference structuredType;

        /// <summary>
        /// Creates a new Edm structured value from an OData resource.
        /// </summary>
        /// <param name="resource">The <see cref="ODataResource"/> to create the structured value for.</param>
        internal ODataEdmStructuredValue(ODataResource resource)
            : base(resource.GetEdmType())
        {
            Debug.Assert(resource != null, "resource != null");

            this.properties = resource.NonComputedProperties;
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
