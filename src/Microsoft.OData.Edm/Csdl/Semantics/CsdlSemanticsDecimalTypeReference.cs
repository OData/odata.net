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
        public CsdlSemanticsDecimalTypeReference(CsdlSemanticsModel model, CsdlDecimalTypeReference reference)
            : base(model, reference)
        {
        }

        public int? Precision => ((CsdlDecimalTypeReference)this.Reference).Precision;

        public int? Scale => ((CsdlDecimalTypeReference)this.Reference).Scale;
    }
}
