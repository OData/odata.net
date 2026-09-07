//---------------------------------------------------------------------
// <copyright file="CsdlTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL type definition.
    /// </summary>
    internal class CsdlTypeDefinition : CsdlNamedElement
    {
        private readonly string underlyingTypeName;
        private readonly int? maxLength;
        private readonly bool? isUnicode;
        private readonly int? precision;
        private readonly int? scale;
        private readonly int? srid;

        public CsdlTypeDefinition(string name, string underlyingTypeName, CsdlLocation location)
            : base(name, location)
        {
            this.underlyingTypeName = underlyingTypeName;
        }

        public CsdlTypeDefinition(
            string name,
            string underlyingTypeName,
            int? maxLength,
            bool? isUnicode,
            int? precision,
            int? scale,
            int? srid,
            CsdlLocation location)
            : this(name, underlyingTypeName, location)
        {
            this.maxLength = maxLength;
            this.isUnicode = isUnicode;
            this.precision = precision;
            this.scale = scale;
            this.srid = srid;
        }

        public string UnderlyingTypeName
        {
            get { return this.underlyingTypeName; }
        }

        public int? MaxLength => this.maxLength;
        public bool? IsUnicode => this.isUnicode;

        public int? Precision => this.precision;

        public int? Scale => this.scale;

        public int? Srid => this.srid;
    }
}
