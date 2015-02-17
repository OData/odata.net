//---------------------------------------------------------------------
// <copyright file="INamedItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System;

    /// <summary>
    /// Interface for an item which has both NamespaceName and Name.
    /// </summary>
    public interface INamedItem : IEquatable<INamedItem>
    {
        /// <summary>
        /// Gets or sets type name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets type namespace.
        /// </summary>
        string NamespaceName { get; set; }

        /// <summary>
        /// Gets the type's full name which includes NamespaceName and Name separated by dot. 
        /// </summary>
        string FullName { get; }
    }
}
