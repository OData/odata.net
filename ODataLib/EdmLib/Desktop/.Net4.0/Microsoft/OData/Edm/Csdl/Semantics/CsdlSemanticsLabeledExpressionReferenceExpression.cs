//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
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
