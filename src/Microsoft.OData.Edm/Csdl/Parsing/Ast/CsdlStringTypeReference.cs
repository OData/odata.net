//---------------------------------------------------------------------
// <copyright file="CsdlStringTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL String type.
    /// </summary>
    internal class CsdlStringTypeReference : CsdlPrimitiveTypeReference
    {
        private readonly bool isUnbounded;
        private readonly int? maxLength;
        private readonly bool? isUnicode;
        private readonly string collation;

        public CsdlStringTypeReference(bool isUnbounded, int? maxLength, bool? isUnicode, string collation, string typeName, bool isNullable, CsdlLocation location)
            : base(EdmPrimitiveTypeKind.String, typeName, isNullable, location)
        {
            this.isUnbounded = isUnbounded;
            this.maxLength = maxLength;
            this.isUnicode = isUnicode;
            this.collation = collation;
        }

        public bool IsUnbounded
        {
            get { return this.isUnbounded; }
        }

        public int? MaxLength
        {
            get { return this.maxLength; }
        }

        public bool? IsUnicode
        {
            get { return this.isUnicode; }
        }

        public string Collation
        {
            get { return this.collation; }
        }
    }
}
