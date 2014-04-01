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
    /// Provides semantics for a CsdlOperation
    /// </summary>
    internal class CsdlSemanticsOperation : CsdlSemanticsFunctionBase, IEdmOperation
    {
        private readonly CsdlOperation operation;

        public CsdlSemanticsOperation(CsdlSemanticsSchema context, CsdlOperation operation)
            : base(context, operation)
        {
            this.operation = operation;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.Context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.operation; }
        }

        public string Namespace
        {
            get { return this.Context.Namespace; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Operation; }
        }
    }
}
