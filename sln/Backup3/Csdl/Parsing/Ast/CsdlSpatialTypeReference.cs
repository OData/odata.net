//---------------------------------------------------------------------
// <copyright file="CsdlSpatialTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlSpatialTypeReference : CsdlPrimitiveTypeReference
    {
        private readonly int?srid;

        public CsdlSpatialTypeReference(EdmPrimitiveTypeKind kind, int? srid, string typeName, bool isNullable, CsdlLocation location)
            : base(kind, typeName, isNullable, location)
        {
            this.srid = srid;
        }

        public int? Srid
        {
            get { return this.srid; }
        }
    }
}
