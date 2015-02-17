//---------------------------------------------------------------------
// <copyright file="AssociationEndReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// AssociationEnd reference
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class AssociationEndReference : AssociationEnd
    {
        /// <summary>
        /// Initializes a new instance of the AssociationEndReference class with given role name.
        /// </summary>
        /// <param name="roleName">The given role name</param>
        public AssociationEndReference(string roleName)
            : base(roleName)
        {
        }
    }
}
