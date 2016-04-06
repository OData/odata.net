//---------------------------------------------------------------------
// <copyright file="IEdmDurationValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM duration value.
    /// </summary>
    public interface IEdmDurationValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the definition of this duration value.
        /// </summary>
        TimeSpan Value { get; }
    }
}
