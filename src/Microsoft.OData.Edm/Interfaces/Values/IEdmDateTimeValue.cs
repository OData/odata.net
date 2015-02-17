//---------------------------------------------------------------------
// <copyright file="IEdmDateTimeValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Values
{
    /// <summary>
    /// Represents an EDM datetime value.
    /// </summary>
    public interface IEdmDateTimeValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the definition of this datetime value.
        /// </summary>
        DateTime Value { get; }
    }
}
