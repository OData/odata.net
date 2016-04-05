//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsParameterReferenceExpression.cs" company="Microsoft">
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
    internal class CsdlSemanticsParameterReferenceExpression : CsdlSemanticsExpression, IEdmParameterReferenceExpression, IEdmCheckable
    {
        private readonly CsdlParameterReferenceExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsParameterReferenceExpression, IEdmOperationParameter> referencedCache = new Cache<CsdlSemanticsParameterReferenceExpression, IEdmOperationParameter>();
        private static readonly Func<CsdlSemanticsParameterReferenceExpression, IEdmOperationParameter> ComputeReferencedFunc = (me) => me.ComputeReferenced();

        public CsdlSemanticsParameterReferenceExpression(CsdlParameterReferenceExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.ParameterReference; }
        }

        public IEdmOperationParameter ReferencedParameter
        {
            get { return this.referencedCache.GetValue(this, ComputeReferencedFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.ReferencedParameter is IUnresolvedElement)
                {
                    return this.ReferencedParameter.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmOperationParameter ComputeReferenced()
        {
            return new UnresolvedParameter(new UnresolvedOperation(String.Empty, Edm.Strings.Bad_UnresolvedOperation(String.Empty), this.Location), this.expression.Parameter, this.Location);
        }
    }
}
