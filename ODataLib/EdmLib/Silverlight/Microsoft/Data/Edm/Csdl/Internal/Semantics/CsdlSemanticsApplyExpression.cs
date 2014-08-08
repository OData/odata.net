//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsApplyExpression : CsdlSemanticsExpression, IEdmApplyExpression, IEdmCheckable
    {
        private readonly CsdlApplyExpression expression;
        private readonly CsdlSemanticsSchema schema;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsApplyExpression, IEdmExpression> appliedFunctionCache = new Cache<CsdlSemanticsApplyExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsApplyExpression, IEdmExpression> ComputeAppliedFunctionFunc = (me) => me.ComputeAppliedFunction();

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

        public IEdmExpression AppliedFunction
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

        private IEdmExpression ComputeAppliedFunction()
        {
            if (this.expression.Function == null)
            {
                return CsdlSemanticsModel.WrapExpression(this.expression.Arguments.FirstOrDefault(null), this.bindingContext, this.schema);
            }
            else
            {
                IEdmFunction referenced;
                IEnumerable<IEdmFunction> candidateFunctions = this.schema.FindFunctions(this.expression.Function);
                int candidateCount = candidateFunctions.Count();
                if (candidateCount == 0)
                {
                    referenced = new UnresolvedFunction(this.expression.Function, Edm.Strings.Bad_UnresolvedFunction(this.expression.Function), this.Location);
                }
                else
                {
                    candidateFunctions = candidateFunctions.Where(this.IsMatchingFunction);
                    candidateCount = candidateFunctions.Count();
                    if (candidateCount > 1)
                    {
                        candidateFunctions = candidateFunctions.Where(this.IsExactMatch);
                        candidateCount = candidateFunctions.Count();
                        if (candidateCount != 1)
                        {
                            referenced = new UnresolvedFunction(this.expression.Function, Edm.Strings.Bad_AmbiguousFunction(this.expression.Function), this.Location);
                        }
                        else
                        {
                            referenced = candidateFunctions.Single();
                        }
                    }
                    else if (candidateCount == 0)
                    {
                        referenced = new UnresolvedFunction(this.expression.Function, Edm.Strings.Bad_FunctionParametersDontMatch(this.expression.Function), this.Location);
                    }
                    else
                    {
                        referenced = candidateFunctions.Single();
                    }
                }

                return new Library.Expressions.EdmFunctionReferenceExpression(referenced);
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

        private bool IsMatchingFunction(IEdmFunction function)
        {
            if (function.Parameters.Count() != this.Arguments.Count())
            {
                return false;
            }

            IEnumerator<IEdmExpression> parameterExpressionEnumerator = this.Arguments.GetEnumerator();
            foreach (IEdmFunctionParameter parameter in function.Parameters)
            {
                parameterExpressionEnumerator.MoveNext();
                IEnumerable<EdmError> recursiveErrors;
                if (!parameterExpressionEnumerator.Current.TryAssertType(parameter.Type, this.bindingContext, false, out recursiveErrors))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsExactMatch(IEdmFunction function)
        {
            IEnumerator<IEdmExpression> parameterExpressionEnumerator = this.Arguments.GetEnumerator();
            foreach (IEdmFunctionParameter parameter in function.Parameters)
            {
                parameterExpressionEnumerator.MoveNext();
                IEnumerable<EdmError> recursiveErrors;
                if (!parameterExpressionEnumerator.Current.TryAssertType(parameter.Type, this.bindingContext, true, out recursiveErrors))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
