//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperationReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsOperationReferenceExpression : CsdlSemanticsExpression, IEdmOperationReferenceExpression, IEdmCheckable
    {
        private readonly CsdlOperationReferenceExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsOperationReferenceExpression, IEdmOperation> referencedCache = new Cache<CsdlSemanticsOperationReferenceExpression, IEdmOperation>();
        private static readonly Func<CsdlSemanticsOperationReferenceExpression, IEdmOperation> ComputeReferencedFunc = (me) => me.ComputeReferenced();

        public CsdlSemanticsOperationReferenceExpression(CsdlOperationReferenceExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.OperationReference; }
        }

        public IEdmOperation ReferencedOperation
        {
            get { return this.referencedCache.GetValue(this, ComputeReferencedFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.ReferencedOperation is IUnresolvedElement)
                {
                    return this.ReferencedOperation.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmOperation ComputeReferenced()
        {
            return new UnresolvedOperation(this.expression.Operation, Edm.Strings.Bad_UnresolvedOperation(this.expression.Operation), this.Location);
        }
    }
}
