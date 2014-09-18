//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for <see cref="CsdlExpressionTypeReference"/>.
    /// </summary>
    internal abstract class CsdlSemanticsTypeExpression : CsdlSemanticsElement, IEdmTypeReference
    {
        private readonly CsdlExpressionTypeReference expressionUsage;
        private readonly CsdlSemanticsTypeDefinition type;

        protected CsdlSemanticsTypeExpression(CsdlExpressionTypeReference expressionUsage, CsdlSemanticsTypeDefinition type)
            : base(expressionUsage)
        {
            this.expressionUsage = expressionUsage;
            this.type = type;
        }

        public IEdmType Definition
        {
            get { return this.type; }
        }

        public bool IsNullable
        {
            get { return this.expressionUsage.IsNullable; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.type.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.expressionUsage; }
        }

        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
