//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Internal;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Path expression.
    /// </summary>
    internal class CsdlSemanticsPathExpression : CsdlSemanticsExpression, IEdmPathExpression
    {
        private readonly CsdlPathExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsPathExpression, IEnumerable<string>> pathCache = new Cache<CsdlSemanticsPathExpression, IEnumerable<string>>();
        private static readonly Func<CsdlSemanticsPathExpression, IEnumerable<string>> ComputePathFunc = (me) => me.ComputePath();

        public CsdlSemanticsPathExpression(CsdlPathExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Path; }
        }

        public IEnumerable<string> Path
        {
            get { return this.pathCache.GetValue(this, ComputePathFunc, null); }
        }

        private IEnumerable<string> ComputePath()
        {
            return this.expression.Path.Split(new char[] { '/' }, StringSplitOptions.None);
        }
    }
}
