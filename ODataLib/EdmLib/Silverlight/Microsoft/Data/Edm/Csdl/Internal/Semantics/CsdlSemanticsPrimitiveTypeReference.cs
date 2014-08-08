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

using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics of a reference to an EDM primitive type.
    /// </summary>
    internal class CsdlSemanticsPrimitiveTypeReference : CsdlSemanticsElement, IEdmPrimitiveTypeReference
    {
        internal readonly CsdlPrimitiveTypeReference Reference;
        private readonly CsdlSemanticsSchema schema;

        /// <summary>
        /// This doesn't need the full caching mechanism because the computation is cheap, and the likelyhood of computing a primitive type reference without needing its definition is remote.
        /// </summary>
        private readonly IEdmPrimitiveType definition;

        public CsdlSemanticsPrimitiveTypeReference(CsdlSemanticsSchema schema, CsdlPrimitiveTypeReference reference)
            : base(reference)
        {
            this.schema = schema;
            this.Reference = reference;
            this.definition = EdmCoreModel.Instance.GetPrimitiveType(this.Reference.Kind); 
        }

        public bool IsNullable
        {
            get { return this.Reference.IsNullable; }
        }

        public IEdmType Definition
        {
            get { return this.definition; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.Reference; }
        }

        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
