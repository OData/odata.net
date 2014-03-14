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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsLabeledExpressionReferenceExpression : CsdlSemanticsExpression, IEdmLabeledExpressionReferenceExpression, IEdmCheckable
    {
        private readonly CsdlLabeledExpressionReferenceExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsLabeledExpressionReferenceExpression, IEdmLabeledExpression> elementCache = new Cache<CsdlSemanticsLabeledExpressionReferenceExpression, IEdmLabeledExpression>();
        private static readonly Func<CsdlSemanticsLabeledExpressionReferenceExpression, IEdmLabeledExpression> ComputeElementFunc = (me) => me.ComputeElement();

        public CsdlSemanticsLabeledExpressionReferenceExpression(CsdlLabeledExpressionReferenceExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.LabeledExpressionReference; }
        }

        public IEdmLabeledExpression ReferencedLabeledExpression
        {
            get { return this.elementCache.GetValue(this, ComputeElementFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.ReferencedLabeledExpression is IUnresolvedElement)
                {
                    return this.ReferencedLabeledExpression.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmLabeledExpression ComputeElement()
        {
            CsdlSemanticsModel model = this.Schema.Model;
            IEdmLabeledExpression result = this.Schema.FindLabeledElement(this.expression.Label, this.bindingContext);
            if (result != null)
            {
                return result;
            }

            return new UnresolvedLabeledElement(this.expression.Label, this.Location);
        }
    }
}
