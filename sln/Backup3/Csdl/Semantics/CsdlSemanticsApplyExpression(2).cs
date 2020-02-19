//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsApplyExpression.cs" company="Microsoft">
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
    internal class CsdlSemanticsApplyExpression : CsdlSemanticsExpression, IEdmApplyExpression, IEdmCheckable
    {
        private readonly CsdlApplyExpression expression;
        private readonly CsdlSemanticsSchema schema;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsApplyExpression, IEdmFunction> appliedFunctionCache = new Cache<CsdlSemanticsApplyExpression, IEdmFunction>();
        private static readonly Func<CsdlSemanticsApplyExpression, IEdmFunction> ComputeAppliedFunctionFunc = (me) => me.ComputeAppliedFunction();

        private readonly Cache<CsdlSemanticsApplyExpression, IEnumerable<IEdmExpression>> argumentsCache = new Cache<CsdlSemanticsApplyExpression, IEnumerable<IEdmExpression>>();
        private static readonly Func<CsdlSemanticsApplyExpression, IEnumerable<IEdmExpression>> ComputeArgumentsFunc = (me) => me.ComputeArguments();

        public CsdlSemanticsApplyExpression(CsdlApplyExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
            this.schema = schema;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.FunctionApplication; }
        }

        public IEdmFunction AppliedFunction
        {
            get { return this.appliedFunctionCache.GetValue(this, ComputeAppliedFunctionFunc, null); }
        }

        public IEnumerable<IEdmExpression> Arguments
        {
            get { return this.argumentsCache.GetValue(this, ComputeArgumentsFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.AppliedFunction is IUnresolvedElement)
                {
                    return this.AppliedFunction.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmFunction ComputeAppliedFunction()
        {
            if (this.expression.Function == null)
            {
                return null;
            }

            IEnumerable<IEdmFunction> candidateFunctions = this.schema.FindOperations(this.expression.Function).OfType<IEdmFunction>();
            int candidateCount = candidateFunctions.Count();
            if (candidateCount == 0)
            {
                return new UnresolvedFunction(this.expression.Function, Edm.Strings.Bad_UnresolvedOperation(this.expression.Function), this.Location);
            }

            candidateFunctions = candidateFunctions.Where(this.IsMatchingFunction);
            candidateCount = candidateFunctions.Count();
            if (candidateCount > 1)
            {
                candidateFunctions = candidateFunctions.Where(this.IsExactMatch);
                candidateCount = candidateFunctions.Count();
                if (candidateCount != 1)
                {
                    return new UnresolvedFunction(this.expression.Function, Edm.Strings.Bad_AmbiguousOperation(this.expression.Function), this.Location);
                }

                return candidateFunctions.Single();
            }

            if (candidateCount == 0)
            {
                return new UnresolvedFunction(this.expression.Function, Edm.Strings.Bad_OperationParametersDontMatch(this.expression.Function), this.Location);
            }

            return candidateFunctions.Single();
        }

        private IEnumerable<IEdmExpression> ComputeArguments()
        {
            bool skipFirst = this.expression.Function == null;
            List<IEdmExpression> result = new List<IEdmExpression>();
            foreach (CsdlExpressionBase argument in this.expression.Arguments)
            {
                if (skipFirst)
                {
                    skipFirst = false;
                }
                else
                {
                    result.Add(CsdlSemanticsModel.WrapExpression(argument, this.bindingContext, this.schema));
                }
            }

            return result;
        }

        private bool IsMatchingFunction(IEdmOperation operation)
        {
            if (operation.Parameters.Count() != this.Arguments.Count())
            {
                return false;
            }

            IEnumerator<IEdmExpression> parameterExpressionEnumerator = this.Arguments.GetEnumerator();
            foreach (IEdmOperationParameter parameter in operation.Parameters)
            {
                parameterExpressionEnumerator.MoveNext();
                IEnumerable<EdmError> recursiveErrors;
                if (!parameterExpressionEnumerator.Current.TryCast(parameter.Type, this.bindingContext, false, out recursiveErrors))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsExactMatch(IEdmOperation operation)
        {
            IEnumerator<IEdmExpression> parameterExpressionEnumerator = this.Arguments.GetEnumerator();
            foreach (IEdmOperationParameter parameter in operation.Parameters)
            {
                parameterExpressionEnumerator.MoveNext();
                IEnumerable<EdmError> recursiveErrors;
                if (!parameterExpressionEnumerator.Current.TryCast(parameter.Type, this.bindingContext, true, out recursiveErrors))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
