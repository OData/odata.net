//---------------------------------------------------------------------
// <copyright file="IEdmGuidValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM integer value.
    /// </summary>
    public interface IEdmGuidValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the definition of this guid value.
        /// </summary>
        Guid Value { get; }
    }
}
