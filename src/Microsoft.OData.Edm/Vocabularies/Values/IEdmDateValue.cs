//---------------------------------------------------------------------
// <copyright file="IEdmDateValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM date.
    /// </summary>
    public interface IEdmDateValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        DateOnly Value { get; }
    }
}
