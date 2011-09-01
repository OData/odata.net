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
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics of a reference to an EDM primitive type.
    /// </summary>
    internal class CsdlSemanticsPrimitiveTypeReference : CsdlSemanticsElement, IEdmPrimitiveTypeReference
    {
        protected readonly CsdlPrimitiveTypeReference reference;
        protected readonly CsdlSemanticsSchema schema;

        public CsdlSemanticsPrimitiveTypeReference(CsdlSemanticsSchema schema, CsdlPrimitiveTypeReference reference)
        {
            this.schema = schema;
            this.reference = reference;
        }

        public bool IsNullable
        {
            get { return this.reference.IsNullable; }
        }

        public IEdmType Definition
        {
            get { return EdmCoreModel.Instance.GetPrimitiveType(this.reference.Kind); }
        }

        public override string ToString()
        {
            return this.ToTraceString();
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.reference; }
        }
    }
}
