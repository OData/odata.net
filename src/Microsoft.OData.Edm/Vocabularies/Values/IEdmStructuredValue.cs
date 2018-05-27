//---------------------------------------------------------------------
// <copyright file="IEdmStructuredValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
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
        /// Finds the value corresponding to the provided property name.
        /// </summary>
        /// <param name="propertyName">Property to find the value of.</param>
        /// <returns>The found property, or null if no property was found.</returns>
        IEdmPropertyValue FindPropertyValue(string propertyName);
    }
}
