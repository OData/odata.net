//---------------------------------------------------------------------
// <copyright file="EndMultiplicity.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Multiplicity for AssociationEnd
    /// </summary>
    public enum EndMultiplicity
    {
        /// <summary>
        /// Multiplicity One (1..1) 
        /// </summary>
        One,

        /// <summary>
        /// Multiplicity ZeroOne (0..1)
        /// </summary>
        ZeroOne,
        
        /// <summary>
        /// Multiplicity Many (*)
        /// </summary>
        Many,
    }
}
