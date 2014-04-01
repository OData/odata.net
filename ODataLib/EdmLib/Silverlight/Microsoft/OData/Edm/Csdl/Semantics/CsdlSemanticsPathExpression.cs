//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Path expression.
    /// </summary>
    internal class CsdlSemanticsPathExpression : CsdlSemanticsExpression, IEdmPathExpression
    {
        protected readonly CsdlPathExpression Expression;

        protected readonly IEdmEntityType BindingContext;

        protected readonly Cache<CsdlSemanticsPathExpression, IEnumerable<string>> PathCache = new Cache<CsdlSemanticsPathExpression, IEnumerable<string>>();

        protected static readonly Func<CsdlSemanticsPathExpression, IEnumerable<string>> ComputePathFunc = (me) => me.ComputePath();

        public CsdlSemanticsPathExpression(CsdlPathExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.Expression = expression;
            this.BindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.Expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Path; }
        }

        public IEnumerable<string> Path
        {
            get { return this.PathCache.GetValue(this, ComputePathFunc, null); }
        }

        private IEnumerable<string> ComputePath()
        {
            return this.Expression.Path.Split(new char[] { '/' }, StringSplitOptions.None);
        }
    }
}
