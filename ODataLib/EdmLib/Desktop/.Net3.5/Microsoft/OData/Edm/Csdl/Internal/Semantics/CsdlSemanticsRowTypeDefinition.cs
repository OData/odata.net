//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlRowType.
    /// </summary>
    internal class CsdlSemanticsRowTypeDefinition : CsdlSemanticsStructuredTypeDefinition, IEdmRowType
    {
        private readonly CsdlRowType row;

        public CsdlSemanticsRowTypeDefinition(CsdlSemanticsSchema context, CsdlRowType row)
            : base(context, row)
        {
            this.row = row;
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Row; }
        }

        public override IEdmStructuredType BaseType
        {
            get { return null; }
        }

        protected override CsdlStructuredType MyStructured
        {
            get { return this.row; }
        }
    }
}
