//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsEnumMemberReferenceExpression : CsdlSemanticsExpression, IEdmEnumMemberReferenceExpression, IEdmCheckable
    {
        private readonly CsdlEnumMemberReferenceExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsEnumMemberReferenceExpression, IEdmEnumMember> referencedCache = new Cache<CsdlSemanticsEnumMemberReferenceExpression, IEdmEnumMember>();
        private static readonly Func<CsdlSemanticsEnumMemberReferenceExpression, IEdmEnumMember> ComputeReferencedFunc = (me) => me.ComputeReferenced();

        public CsdlSemanticsEnumMemberReferenceExpression(CsdlEnumMemberReferenceExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.EnumMemberReference; }
        }

        public IEdmEnumMember ReferencedEnumMember
        {
            get { return this.referencedCache.GetValue(this, ComputeReferencedFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.ReferencedEnumMember is IUnresolvedElement)
                {
                    return this.ReferencedEnumMember.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmEnumMember ComputeReferenced()
        {
            string[] path = this.expression.EnumMemberPath.Split('/');
            Debug.Assert(path.Count() == 2, "Enum member path was not correctly split");

            return new UnresolvedEnumMember(path[1], new UnresolvedEnumType(path[0], this.Location), this.Location);
        }
    }
}
