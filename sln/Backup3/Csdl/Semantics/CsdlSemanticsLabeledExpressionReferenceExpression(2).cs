//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsLabeledExpressionReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

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
            IEdmLabeledExpression result = this.Schema.FindLabeledElement(this.expression.Label, this.bindingContext);
            if (result != null)
            {
                return result;
            }

            return new UnresolvedLabeledElement(this.expression.Label, this.Location);
        }
    }
}
