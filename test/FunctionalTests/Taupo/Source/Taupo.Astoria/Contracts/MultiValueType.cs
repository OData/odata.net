//---------------------------------------------------------------------
// <copyright file="MultiValueType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;

    /// <summary>
    /// Represents a enum for selecting types of MultiValueProperties
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
    public enum MultiValueType
    {
        /// <summary>
        /// ComplexMultiValue Property
        /// </summary>
        Complex = 0,
        
        /// <summary>
        /// PrimitiveMultiValue Property
        /// </summary>
        Primitive = 1,
    }
}