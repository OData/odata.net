//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
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
