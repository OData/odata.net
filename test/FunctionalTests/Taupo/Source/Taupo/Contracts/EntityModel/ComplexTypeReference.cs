//---------------------------------------------------------------------
// <copyright file="ComplexTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Reference to a complex type defined elsewhere.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public sealed class ComplexTypeReference : ComplexType
    {
        /// <summary>
        /// Initializes a new instance of the ComplexTypeReference class with given name.
        /// </summary>
        /// <param name="name">Type reference name</param>
        public ComplexTypeReference(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComplexTypeReference class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">Type namespace</param>
        /// <param name="name">Type reference name</param>
        public ComplexTypeReference(string namespaceName, string name)
            : base(namespaceName, name)
        {
        }
    }
}
