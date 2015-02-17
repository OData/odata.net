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
    }
}
