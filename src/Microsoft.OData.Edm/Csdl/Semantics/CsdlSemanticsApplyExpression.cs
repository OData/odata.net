//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsApplyExpression.cs" company="Microsoft">
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
    internal class CsdlSemanticsApplyExpression : CsdlSemanticsExpression, IEdmApplyExpression, IEdmCheckable
    {
        private readonly CsdlApplyExpression expression;
        private readonly CsdlSemanticsSchema schema;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsApplyExpression, IEdmExpression> appliedOperationCache = new Cache<CsdlSemanticsApplyExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsApplyExpression, IEdmExpression> ComputeAppliedOperationFunc = (me) => me.ComputeAppliedOperation();

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
            get { return EdmExpressionKind.OperationApplication; }
        }

        public IEdmExpression AppliedOperation
        {
            get { return this.appliedOperationCache.GetValue(this, ComputeAppliedOperationFunc, null); }
        }

        public IEnumerable<IEdmExpression> Arguments
        {
            get { return this.argumentsCache.GetValue(this, ComputeArgumentsFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.AppliedOperation is IUnresolvedElement)
                {
                    return this.AppliedOperation.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmExpression ComputeAppliedOperation()
        {
            if (this.expression.Function == null)
            {
                return CsdlSemanticsModel.WrapExpression(this.expression.Arguments.FirstOrDefault(null), this.bindingContext, this.schema);
            }
            else
            {
                IEdmOperation referenced;
                IEnumerable<IEdmOperation> candidateOperations = this.schema.FindOperations(this.expression.Function);
                int candidateCount = candidateOperations.Count();
                if (candidateCount == 0)
                {
                    referenced = new UnresolvedOperation(this.expression.Function, Edm.Strings.Bad_UnresolvedOperation(this.expression.Function), this.Location);
                }
                else
                {
                    candidateOperations = candidateOperations.Where(this.IsMatchingFunction);
                    candidateCount = candidateOperations.Count();
                    if (candidateCount > 1)
                    {
                        candidateOperations = candidateOperations.Where(this.IsExactMatch);
                        candidateCount = candidateOperations.Count();
                        if (candidateCount != 1)
                        {
                            referenced = new UnresolvedOperation(this.expression.Function, Edm.Strings.Bad_AmbiguousOperation(this.expression.Function), this.Location);
                        }
                        else
                        {
                            referenced = candidateOperations.Single();
                        }
                    }
                    else if (candidateCount == 0)
                    {
                        referenced = new UnresolvedOperation(this.expression.Function, Edm.Strings.Bad_OperationParametersDontMatch(this.expression.Function), this.Location);
                    }
                    else
                    {
                        referenced = candidateOperations.Single();
                    }
                }

                return new Library.Expressions.EdmOperationReferenceExpression(referenced);
            }
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
