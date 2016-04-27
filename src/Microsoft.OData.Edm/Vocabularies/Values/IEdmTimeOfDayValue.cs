//---------------------------------------------------------------------
// <copyright file="IEdmTimeOfDayValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM TimeOfDay value.
    /// </summary>
    public interface IEdmTimeOfDayValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        TimeOfDay Value { get; }
    }
}
