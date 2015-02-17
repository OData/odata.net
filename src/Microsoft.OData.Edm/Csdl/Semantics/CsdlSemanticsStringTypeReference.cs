//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsStringTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics of a reference to an EDM String type.
    /// </summary>
    internal class CsdlSemanticsStringTypeReference : CsdlSemanticsPrimitiveTypeReference, IEdmStringTypeReference
    {
        public CsdlSemanticsStringTypeReference(CsdlSemanticsSchema schema, CsdlStringTypeReference reference)
            : base(schema, reference)
        {
        }

        public bool IsUnbounded
        {
            get { return ((CsdlStringTypeReference)this.Reference).IsUnbounded; }
        }

        public int? MaxLength
        {
            get { return ((CsdlStringTypeReference)this.Reference).MaxLength; }
        }

        public bool? IsUnicode
        {
            get { return ((CsdlStringTypeReference)this.Reference).IsUnicode; }
        }
    }
}
