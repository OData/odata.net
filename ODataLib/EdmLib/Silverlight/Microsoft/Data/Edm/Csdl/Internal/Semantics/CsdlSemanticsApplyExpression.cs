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
