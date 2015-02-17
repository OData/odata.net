//---------------------------------------------------------------------
// <copyright file="MultiplicityPattern.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Specifies the <see cref="EndMultiplicity"/> pattern for both sides of a <see cref="RelationshipType"/>.
    /// </summary>
    public enum MultiplicityPattern
    {
        /// <summary>
        /// Both sides of the relationship can take many instances. (*:*)
        /// </summary>
        ManyToMany,

        /// <summary>
        /// One side of the relationship takes many instances and the other side is required. (*:0..1)
        /// </summary>
        ManyToOne,

        /// <summary>
        /// One side of the relationship takes many instances and the other side is optional. (*:0..1)
        /// </summary>
        ManyToZeroOne,

        /// <summary>
        /// Both sides of the relationship are required. (1:1)
        /// </summary>
        OneToOne,

        /// <summary>
        /// One side of the relationship is required, and the other side is optional. (1:0..1)
        /// </summary>
        OneToZeroOne,

        /// <summary>
        /// Both sides of the relationship are optional. (0..1:0..1)
        /// </summary>
        ZeroOneToZeroOne
    }
}
