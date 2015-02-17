//---------------------------------------------------------------------
// <copyright file="EntityTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Reference to entity type defined elsewhere.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public sealed class EntityTypeReference : EntityType
    {
        /// <summary>
        /// Initializes a new instance of the EntityTypeReference class with a given name.
        /// </summary>
        /// <param name="name">Entity type reference name</param>
        public EntityTypeReference(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntityTypeReference class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">Entity type namespace.</param>
        /// <param name="name">Entity type reference name</param>
        public EntityTypeReference(string namespaceName, string name)
            : base(namespaceName, name)
        {
        }
    }
}
