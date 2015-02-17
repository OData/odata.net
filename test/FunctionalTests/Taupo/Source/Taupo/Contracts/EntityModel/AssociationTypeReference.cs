//---------------------------------------------------------------------
// <copyright file="AssociationTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Association type reference
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class AssociationTypeReference : AssociationType
    {
        /// <summary>
        /// Initializes a new instance of the AssociationTypeReference class with given name.
        /// </summary>
        /// <param name="name">the given name</param>
        public AssociationTypeReference(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssociationTypeReference class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">the given namespace name</param>
        /// <param name="name">the given name</param>
        public AssociationTypeReference(string namespaceName, string name)
            : base(namespaceName, name)
        {
        }
    }
}
