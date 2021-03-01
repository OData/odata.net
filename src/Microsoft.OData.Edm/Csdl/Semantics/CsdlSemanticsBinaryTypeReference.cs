//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsBinaryTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics of a reference to an EDM Binary type.
    /// </summary>
    internal class CsdlSemanticsBinaryTypeReference : CsdlSemanticsPrimitiveTypeReference, IEdmBinaryTypeReference
    {
        public CsdlSemanticsBinaryTypeReference(CsdlSemanticsModel model, CsdlBinaryTypeReference reference)
            : base(model, reference)
        {
        }

        public bool IsUnbounded => ((CsdlBinaryTypeReference)Reference).IsUnbounded;

        public int? MaxLength => ((CsdlBinaryTypeReference)Reference).MaxLength;
    }
}
