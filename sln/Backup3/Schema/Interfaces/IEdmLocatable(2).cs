//---------------------------------------------------------------------
// <copyright file="IEdmLocatable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Interface for all EDM elements that can be located.
    /// </summary>
    public interface IEdmLocatable
    {
        /// <summary>
        /// Gets the location of this element.
        /// </summary>
        EdmLocation Location { get; }
    }
}
