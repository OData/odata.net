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
        public CsdlSemanticsStringTypeReference(CsdlSemanticsModel model, CsdlStringTypeReference reference)
            : base(model, reference)
        {
        }

        public bool IsUnbounded => ((CsdlStringTypeReference)Reference).IsUnbounded;

        public int? MaxLength => ((CsdlStringTypeReference)Reference).MaxLength;

        public bool? IsUnicode => ((CsdlStringTypeReference)Reference).IsUnicode;
    }
}
