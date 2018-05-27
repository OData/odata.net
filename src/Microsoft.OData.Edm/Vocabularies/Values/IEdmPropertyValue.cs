//---------------------------------------------------------------------
// <copyright file="IEdmPropertyValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents a value of an EDM property.
    /// </summary>
    public interface IEdmPropertyValue : IEdmDelayedValue
    {
        /// <summary>
        /// Gets the name of the property this value is associated with.
        /// </summary>
        string Name { get; }
    }
}
