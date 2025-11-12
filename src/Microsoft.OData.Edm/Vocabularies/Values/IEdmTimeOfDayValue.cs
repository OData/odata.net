//---------------------------------------------------------------------
// <copyright file="IEdmTimeOfDayValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

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
        TimeOnly Value { get; }
    }
}
