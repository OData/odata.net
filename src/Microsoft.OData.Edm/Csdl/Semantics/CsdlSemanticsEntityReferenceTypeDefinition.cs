//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEntityReferenceTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlEntityTypeReference.
    /// </summary>
    internal class CsdlSemanticsEntityReferenceTypeDefinition : CsdlSemanticsTypeDefinition, IEdmEntityReferenceType
    {
        private readonly Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> entityTypeCache = new Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType>();
        private static readonly Func<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> ComputeEntityTypeFunc = (me) => me.ComputeEntityType();

        private readonly CsdlEntityReferenceType entityTypeReference;

        public CsdlSemanticsEntityReferenceTypeDefinition(CsdlSemanticsModel model, CsdlEntityReferenceType entityTypeReference)
            : base(entityTypeReference)
        {
            Model = model;
            this.entityTypeReference = entityTypeReference;
        }

        public override EdmTypeKind TypeKind => EdmTypeKind.EntityReference;

        public IEdmEntityType EntityType
        {
            get { return this.entityTypeCache.GetValue(this, ComputeEntityTypeFunc, null); }
        }

        public override CsdlElement Element => this.entityTypeReference;

        public override CsdlSemanticsModel Model { get; }

        private IEdmEntityType ComputeEntityType()
        {
            IEdmTypeReference type = CsdlSemanticsModel.WrapTypeReference(Model, this.entityTypeReference.EntityType);
            return type.TypeKind() == EdmTypeKind.Entity ? type.AsEntity().EntityDefinition() : new UnresolvedEntityType(this.Model.ReplaceAlias(type.FullName()), this.Location);
        }
    }
}
