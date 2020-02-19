//---------------------------------------------------------------------
// <copyright file="EdmStructuredValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM structured value.
    /// </summary>
    public class EdmStructuredValue : EdmValue, IEdmStructuredValue
    {
        private readonly IEnumerable<IEdmPropertyValue> propertyValues;

        private readonly Cache<EdmStructuredValue, Dictionary<string, IEdmPropertyValue>> propertiesDictionaryCache;
        private static readonly Func<EdmStructuredValue, Dictionary<string, IEdmPropertyValue>> ComputePropertiesDictionaryFunc = (me) => me.ComputePropertiesDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuredValue"/> class.
        /// </summary>
        /// <param name="type">Type that describes this value.</param>
        /// <param name="propertyValues">Child values of this value.</param>
        public EdmStructuredValue(IEdmStructuredTypeReference type, IEnumerable<IEdmPropertyValue> propertyValues)
            : base(type)
        {
            EdmUtil.CheckArgumentNull(propertyValues, "propertyValues");

            this.propertyValues = propertyValues;
            if (propertyValues != null)
            {
                // If there are enough property values, make FindPropertyValue use a dictionary.
                int propertyCount = propertyValues.Count();
                if (propertyCount > 5)
                {
                    this.propertiesDictionaryCache = new Cache<EdmStructuredValue, Dictionary<string, IEdmPropertyValue>>();
                }
            }
        }

        /// <summary>
        /// Gets the property values of this structured value.
        /// </summary>
        public IEnumerable<IEdmPropertyValue> PropertyValues
        {
            get { return this.propertyValues; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Structured; }
        }

        private Dictionary<string, IEdmPropertyValue> PropertiesDictionary
        {
            get { return this.propertiesDictionaryCache == null ? null : this.propertiesDictionaryCache.GetValue(this, ComputePropertiesDictionaryFunc, null); }
        }

        /// <summary>
        /// Retrieves the value corresponding to the given property name. Returns null if no such value exists.
        /// </summary>
        /// <param name="propertyName">The property that describes the value being found.</param>
        /// <returns>The requested value, or null if no such value exists.</returns>
        public IEdmPropertyValue FindPropertyValue(string propertyName)
        {
            // If there is a dictionary, use it.
            Dictionary<string, IEdmPropertyValue> propertiesDictionary = this.PropertiesDictionary;
            if (propertiesDictionary != null)
            {
                IEdmPropertyValue propertyValue;
                propertiesDictionary.TryGetValue(propertyName, out propertyValue);
                return propertyValue;
            }

            // If there is no dictionary, go through the property values and take the first match.
            foreach (IEdmPropertyValue propertyValue in this.propertyValues)
            {
                if (propertyValue.Name == propertyName)
                {
                    return propertyValue;
                }
            }

            return null;
        }

        private Dictionary<string, IEdmPropertyValue> ComputePropertiesDictionary()
        {
            Dictionary<string, IEdmPropertyValue> propertiesDictionary = new Dictionary<string, IEdmPropertyValue>();

            foreach (IEdmPropertyValue propertyValue in this.propertyValues)
            {
                propertiesDictionary[propertyValue.Name] = propertyValue;
            }

            return propertiesDictionary;
        }
    }
}
