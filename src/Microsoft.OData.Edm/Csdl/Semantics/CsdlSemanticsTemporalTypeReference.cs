//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsTemporalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics of a reference to an EDM temporal type.
    /// </summary>
    internal class CsdlSemanticsTemporalTypeReference : CsdlSemanticsPrimitiveTypeReference, IEdmTemporalTypeReference
    {
        public CsdlSemanticsTemporalTypeReference(CsdlSemanticsSchema schema, CsdlTemporalTypeReference reference)
            : base(schema, reference)
        {
        }

        public int? Precision
        {
            get { return ((CsdlTemporalTypeReference)this.Reference).Precision; }
        }
    }
}
