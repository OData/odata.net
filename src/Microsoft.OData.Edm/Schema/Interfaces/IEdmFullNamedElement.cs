//---------------------------------------------------------------------
// <copyright file="IEdmFullNamedElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Common base interface for all named EDM elements.
    /// </summary>
    public interface IEdmFullNamedElement : IEdmNamedElement
    {
        /// <summary>
        /// Gets the full name of this element.
        /// </summary>
        string FullName { get; }
    }
}
