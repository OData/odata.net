//---------------------------------------------------------------------
// <copyright file="QualifiedDatabaseTypeNameFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Qualified name of the database type (including facets like precision or scale).
    /// </summary>
    public class QualifiedDatabaseTypeNameFacet : PrimitiveDataTypeFacet<string>
    {
        /// <summary>
        /// Initializes a new instance of the QualifiedDatabaseTypeNameFacet class.
        /// </summary>
        /// <param name="qualifiedTypeName">Qualified name of the type.</param>
        public QualifiedDatabaseTypeNameFacet(string qualifiedTypeName)
            : base(qualifiedTypeName)
        {
        }
    }
}
