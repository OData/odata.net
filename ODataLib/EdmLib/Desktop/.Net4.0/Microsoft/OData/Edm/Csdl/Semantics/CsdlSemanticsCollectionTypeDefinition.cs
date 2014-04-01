//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
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
