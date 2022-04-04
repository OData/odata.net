//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsDecimalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics of a reference to an EDM Decimal type.
    /// </summary>
    internal class CsdlSemanticsDecimalTypeReference : CsdlSemanticsPrimitiveTypeReference, IEdmDecimalTypeReference
    {
        public CsdlSemanticsDecimalTypeReference(CsdlSemanticsSchema schema, CsdlDecimalTypeReference reference)
            : base(schema, reference)
        {
        }

        public int? Precision
        {
            get { return ((CsdlDecimalTypeReference)this.Reference).Precision; }
        }

        public int? Scale
        {
            get { return ((CsdlDecimalTypeReference)this.Reference).Scale; }
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
        public bool ShouldWriteDefaultScale => ((CsdlDecimalTypeReference)this.Reference).ShouldWriteDefaultScale;
    }
}
