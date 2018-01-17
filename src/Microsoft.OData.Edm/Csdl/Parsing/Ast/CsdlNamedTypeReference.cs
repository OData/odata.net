//---------------------------------------------------------------------
// <copyright file="CsdlNamedTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL type reference based on a type referenced by name.
    /// </summary>
    internal class CsdlNamedTypeReference : CsdlTypeReference
    {
        public CsdlNamedTypeReference(string fullName, bool isNullable, CsdlLocation location)
            : this(false, null, null, null, null, null, fullName, isNullable, location)
        {
        }

        public CsdlNamedTypeReference(
            bool isUnbounded,
            int? maxLength,
            bool? isUnicode,
            int? precision,
            int? scale,
            int? spatialReferenceIdentifier,
            string fullName,
            bool isNullable,
            CsdlLocation location)
            : base(isNullable, location)
        {
            this.IsUnbounded = isUnbounded;
            this.MaxLength = maxLength;
            this.IsUnicode = isUnicode;
            this.Precision = precision;
            this.Scale = scale;
            this.SpatialReferenceIdentifier = spatialReferenceIdentifier;
            this.FullName = fullName;
        }

        public bool IsUnbounded { get; protected set; }

        public int? MaxLength { get; protected set; }

        public bool? IsUnicode { get; protected set; }

        public int? Precision { get; protected set; }

        public int? Scale { get; protected set; }

        public int? SpatialReferenceIdentifier { get; protected set; }

        public string FullName { get; protected set; }
    }
}
