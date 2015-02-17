//---------------------------------------------------------------------
// <copyright file="AssociationSetEndReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Reference to association set role.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class AssociationSetEndReference : AssociationSetEnd
    {
        /// <summary>
        /// Initializes a new instance of the AssociationSetEndReference class.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        public AssociationSetEndReference(string roleName)
        {
            this.RoleName = roleName;
        }

        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        public string RoleName { get; private set; }
    }
}
