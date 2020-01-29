//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsSpatialTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsSpatialTypeReference : CsdlSemanticsPrimitiveTypeReference, IEdmSpatialTypeReference
    {
        public CsdlSemanticsSpatialTypeReference(CsdlSemanticsSchema schema, CsdlSpatialTypeReference reference)
            : base(schema, reference)
        {
        }

        public int? SpatialReferenceIdentifier
        {
            get { return ((CsdlSpatialTypeReference)this.Reference).Srid; }
        }
    }
}
