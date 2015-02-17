//---------------------------------------------------------------------
// <copyright file="AccessModifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// CodeGen Access Modifier
    /// </summary>
    public enum AccessModifier
    {
        /// <summary>
        /// Nothing specified
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Private: accessible only within the body of the class or the struct in which they are declared
        /// </summary>
        Private,

        /// <summary>
        /// Internal : accessible only within files in the same assembly
        /// </summary>
        Internal,

        /// <summary>
        /// Protected: accessible from within the class in which it is declared, and from within any class derived from the class that declared this member.
        /// </summary>
        Protected,

        /// <summary>
        /// Public: no restrictions on accessing
        /// </summary>
        Public,
    }
}
