//---------------------------------------------------------------------
// <copyright file="IEdmCollectionValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM collection value.
    /// </summary>
    public interface IEdmCollectionValue : IEdmValue
    {
        /// <summary>
        /// Gets the values stored in this collection.
        /// </summary>
        IEnumerable<IEdmDelayedValue> Elements { get; }
    }
}
