//---------------------------------------------------------------------
// <copyright file="EnumTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Reference to enum type defined elsewhere.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public sealed class EnumTypeReference : EnumType
    {
        /// <summary>
        /// Initializes a new instance of the EnumTypeReference class with a given name.
        /// </summary>
        /// <param name="name">Enum type reference name</param>
        public EnumTypeReference(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EnumTypeReference class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">Enum type namespace.</param>
        /// <param name="name">Enum type reference name</param>
        public EnumTypeReference(string namespaceName, string name)
            : base(namespaceName, name)
        {
        }
    }
}
