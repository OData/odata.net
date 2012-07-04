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
using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal abstract class CsdlSemanticsExpression : CsdlSemanticsElement, IEdmExpression
    {
        private readonly CsdlSemanticsSchema schema;

        protected CsdlSemanticsExpression(CsdlSemanticsSchema schema, CsdlExpressionBase element)
            : base(element)
        {
            this.schema = schema;
        }

        public abstract EdmExpressionKind ExpressionKind
        {
            get;
        }

        public CsdlSemanticsSchema Schema
        {
            get { return this.schema; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }
    }
}
