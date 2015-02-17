//---------------------------------------------------------------------
// <copyright file="AssociationSetReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Association set reference
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class AssociationSetReference : AssociationSet
    {
        /// <summary>
        /// Initializes a new instance of the AssociationSetReference class.
        /// </summary>
        /// <param name="associationSetName">Name of the association set.</param>
        public AssociationSetReference(string associationSetName)
            : base(associationSetName, null)
        {
        }
    }
}
