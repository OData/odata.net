//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsCollectionTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlCollectionType.
    /// </summary>
    internal class CsdlSemanticsCollectionTypeDefinition : CsdlSemanticsTypeDefinition, IEdmCollectionType
    {
        private readonly CsdlCollectionType collection;

        private readonly Cache<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference> elementTypeCache = new Cache<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference> ComputeElementTypeFunc = (me) => me.ComputeElementType();

        public CsdlSemanticsCollectionTypeDefinition(CsdlSemanticsModel model, CsdlCollectionType collection)
            : base(collection)
        {
            Model = model;
            this.collection = collection;
        }

        public override EdmTypeKind TypeKind => EdmTypeKind.Collection;

        public IEdmTypeReference ElementType
        {
            get { return this.elementTypeCache.GetValue(this, ComputeElementTypeFunc, null); }
        }

        public override CsdlSemanticsModel Model { get; }

        public override CsdlElement Element => this.collection;

        private IEdmTypeReference ComputeElementType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.Model, this.collection.ElementType);
        }
    }
}
