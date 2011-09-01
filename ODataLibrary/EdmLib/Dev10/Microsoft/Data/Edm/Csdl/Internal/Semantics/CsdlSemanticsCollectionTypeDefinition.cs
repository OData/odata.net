//   Copyright 2011 Microsoft Corporation
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

using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlMultiValueType.
    /// </summary>
    internal class CsdlSemanticsCollectionTypeDefinition : CsdlSemanticsTypeDefinition, IEdmCollectionType
    {
        private readonly CsdlSemanticsSchema schema;
        private readonly CsdlCollectionType collection;

        public CsdlSemanticsCollectionTypeDefinition(CsdlSemanticsSchema schema, CsdlCollectionType collection)
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
            get { return CsdlSemanticsModel.WrapTypeReference(this.schema, this.collection.ElementType); }
        }

        public bool IsAtomic
        {
            get { return this.collection.IsAtomic; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.collection; }
        }
    }
}
