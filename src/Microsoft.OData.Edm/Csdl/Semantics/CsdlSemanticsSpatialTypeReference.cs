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
        public CsdlSemanticsSpatialTypeReference(CsdlSemanticsModel model, CsdlSpatialTypeReference reference)
            : base(model, reference)
        {
        }

        public int? SpatialReferenceIdentifier => ((CsdlSpatialTypeReference)Reference).Srid;
    }
}
