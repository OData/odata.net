//---------------------------------------------------------------------
// <copyright file="SridFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Specifies the srid of spatial values.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Srid", Justification = "This name is used in the product as well")]
    public class SridFacet : PrimitiveDataTypeFacet<string>
    {
        /// <summary>
        /// Initializes a new instance of the SridFacet class.
        /// </summary>
        /// <param name="srid">The SRID.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Srid", Justification = "This name is used in the product as well")]
        public SridFacet(string srid)
            : base(srid)
        {
        }
    }
}
