//---------------------------------------------------------------------
// <copyright file="CsdlDecimalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL decimal type.
    /// </summary>
    internal class CsdlDecimalTypeReference : CsdlPrimitiveTypeReference
    {
        public CsdlDecimalTypeReference(int? precision, int? scale, string typeName, bool isNullable, CsdlLocation location, bool shouldWriteDefaultScale = false)
            : base(EdmPrimitiveTypeKind.Decimal, typeName, isNullable, location)
        {
            this.Precision = precision;
            this.Scale = scale;
            this.ShouldWriteDefaultScale = shouldWriteDefaultScale;
        }

        /// <summary>
        /// Specifies whether the default scale should be written out explicitly
        /// when the model is being serialized into a CSDL (XML) document.
        /// </summary>
        /// <remarks>
        /// Usually the attribute is not written when it contains the default value.
        /// This flag is only here for backwards compatibility:
        /// Up to v7.10.0, the default scale was 0. This is consistent with the XML CSDL spec (but not JSON spec),
        /// see https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#sec_Scale
        /// However, later versions changed the default scale from 0 to null:
        /// see PR: https://github.com/OData/odata.net/pull/2346
        /// When we parse an XML CSDL document that contains an explicit Scale="variable" into an IEdmModel,
        /// we want to ensure that serializing the IEdmModel back into CSDL will also explicitly write
        /// Scale="variable" for compatibility. In that case this flag should be set to true.
        /// </remarks>
        public bool ShouldWriteDefaultScale { get; private set; }
    }
}
