//---------------------------------------------------------------------
// <copyright file="IEdmDelayedValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents a lazily computed value.
    /// </summary>
    public interface IEdmDelayedValue
    {
        /// <summary>
        /// Gets the data stored in this value.
        /// </summary>
        IEdmValue Value { get; }
    }
}
