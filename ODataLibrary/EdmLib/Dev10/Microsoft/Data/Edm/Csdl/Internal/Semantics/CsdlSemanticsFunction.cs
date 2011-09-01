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
    /// Provides semantics for a CsdlFunction.
    /// </summary>
    internal class CsdlSemanticsFunction : CsdlSemanticsFunctionBase, IEdmFunction
    {
        private readonly CsdlSemanticsSchema context;
        private readonly CsdlFunction function;

        public CsdlSemanticsFunction(CsdlSemanticsSchema context, CsdlFunction function)
            :base(context, function)
        {
            this.context = context;
            this.function = function;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.function; }
        }

        public string Namespace
        {
            get { return this.context.Namespace; }
        }

        public string DefiningExpression
        {
            get { return this.function.DefiningExpression; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }
    }
}
