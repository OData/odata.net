//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlCollectionType.
    /// </summary>
    internal class CsdlSemanticsCollectionTypeDefinition : CsdlSemanticsTypeDefinition, IEdmCollectionType
    {
        private readonly CsdlSemanticsSchema schema;
        private readonly CsdlCollectionType collection;

        private readonly Cache<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference> elementTypeCache = new Cache<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference> ComputeElementTypeFunc = (me) => me.ComputeElementType();

        public CsdlSemanticsCollectionTypeDefinition(CsdlSemanticsSchema schema, CsdlCollectionType collection)
            : base(collection)
        {
            this.collection = collection;
            this.schema = schema;
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Collection; }
        }

        public IEdmTypeReference ElementType
        {
            get { return this.elementTypeCache.GetValue(this, ComputeElementTypeFunc, null); }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.collection; }
        }

        private IEdmTypeReference ComputeElementType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.schema, this.collection.ElementType);
        }
    }
}
