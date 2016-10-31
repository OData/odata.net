//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlEntityTypeReference.
    /// </summary>
    internal class CsdlSemanticsEntityReferenceTypeDefinition : CsdlSemanticsTypeDefinition, IEdmEntityReferenceType
    {
        private readonly CsdlSemanticsSchema schema;

        private readonly Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> entityTypeCache = new Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType>();
        private static readonly Func<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> ComputeEntityTypeFunc = (me) => me.ComputeEntityType();

        private readonly CsdlEntityReferenceType entityTypeReference;

        public CsdlSemanticsEntityReferenceTypeDefinition(CsdlSemanticsSchema schema, CsdlEntityReferenceType entityTypeReference)
            : base(entityTypeReference)
        {
            this.schema = schema;
            this.entityTypeReference = entityTypeReference;
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.EntityReference; }
        }

        public IEdmEntityType EntityType
        {
            get { return this.entityTypeCache.GetValue(this, ComputeEntityTypeFunc, null); }
        }

        public override CsdlElement Element
        {
            get { return this.entityTypeReference; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        private IEdmEntityType ComputeEntityType()
        {
            IEdmTypeReference type = CsdlSemanticsModel.WrapTypeReference(this.schema, this.entityTypeReference.EntityType);
            return type.TypeKind() == EdmTypeKind.Entity ? type.AsEntity().EntityDefinition() : new UnresolvedEntityType(this.schema.UnresolvedName(type.FullName()), this.Location);
        }
    }
}
