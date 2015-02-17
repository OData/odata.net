//---------------------------------------------------------------------
// <copyright file="UnqualifiedDatabaseTypeNameFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Unqualified name of the database type (without qualifiers such as precision or scale).
    /// </summary>
    public class UnqualifiedDatabaseTypeNameFacet : PrimitiveDataTypeFacet<string>
    {
        /// <summary>
        /// Initializes a new instance of the UnqualifiedDatabaseTypeNameFacet class.
        /// </summary>
        /// <param name="unqualifiedDatabaseTypeName">The value.</param>
        public UnqualifiedDatabaseTypeNameFacet(string unqualifiedDatabaseTypeName)
            : base(unqualifiedDatabaseTypeName)
        {
        }
    }
}
