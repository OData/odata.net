//---------------------------------------------------------------------
// <copyright file="IEdmNamedElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Common base interface for all named EDM elements.
    /// </summary>
    public interface IEdmNamedElement : IEdmElement
    {
        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        string Name { get; }
    }
}
