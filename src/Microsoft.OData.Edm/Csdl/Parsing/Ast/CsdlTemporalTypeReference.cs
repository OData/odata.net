//---------------------------------------------------------------------
// <copyright file="CsdlTemporalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL temporal type.
    /// </summary>
    internal class CsdlTemporalTypeReference : CsdlPrimitiveTypeReference
    {
        private readonly int?precision;

        public CsdlTemporalTypeReference(EdmPrimitiveTypeKind kind, int? precision, string typeName, bool isNullable, CsdlLocation location)
            : base(kind, typeName, isNullable, location)
        {
            this.precision = precision;
        }

        public int? Precision
        {
            get { return this.precision; }
        }
    }
}
