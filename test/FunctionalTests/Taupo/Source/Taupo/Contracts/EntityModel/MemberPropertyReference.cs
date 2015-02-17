//---------------------------------------------------------------------
// <copyright file="MemberPropertyReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Represents a reference to a <see cref="MemberProperty"/>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class MemberPropertyReference : MemberProperty
    {
        /// <summary>
        /// Initializes a new instance of the MemberPropertyReference class with given property name.
        /// </summary>
        /// <param name="name">Property name</param>
        public MemberPropertyReference(string name)
            : base(name)
        {
        }
    }
}
