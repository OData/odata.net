//---------------------------------------------------------------------
// <copyright file="IEdmPropertyConstructor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM property constructor specified as part of a EDM construction record expression.
    /// </summary>
    public interface IEdmPropertyConstructor : IEdmElement
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the expression for the value of the property.
        /// </summary>
        IEdmExpression Value { get; }
    }
}
