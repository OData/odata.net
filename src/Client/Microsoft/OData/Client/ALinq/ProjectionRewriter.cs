//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using Microsoft.OData.Client.Metadata;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    #endregion Namespaces

    internal class ProjectionRewriter : ALinqExpressionVisitor
    {
        #region Private fields

        private readonly ParameterExpression newLambdaParameter;

        private ParameterExpression oldLambdaParameter;
        private ResourceExpression projectionSource;

        private bool successfulRebind;

        #endregion Private fields

        private ProjectionRewriter(Type proposedParameterType)
        {
            Debug.Assert(proposedParameterType != null, "proposedParameterType != null");
            this.newLambdaParameter = Expression.Parameter(proposedParameterType, "it");
        }

        #region Internal methods.

        internal static LambdaExpression TryToRewrite(LambdaExpression le, ResourceExpression source)
        {
            Type proposedParameterType = source.ResourceType;
            LambdaExpression result;
            if (!ResourceBinder.PatternRules.MatchSingleArgumentLambda(le, out le) ||  // can only rewrite single parameter Lambdas.
                ClientTypeUtil.TypeOrElementTypeIsEntity(le.Parameters[0].Type) || // only attempt to rewrite if lambda parameter is not an entity type
                !(le.Parameters[0].Type.GetProperties().Any(p => p.PropertyType == proposedParameterType))) // lambda parameter must have public property that is same as proposed type.
            {
                result = le;
            }
            else
            {
                ProjectionRewriter rewriter = new ProjectionRewriter(proposedParameterType);
                result = rewriter.Rebind(le, source);
            }

            return result;
        }

        internal LambdaExpression Rebind(LambdaExpression lambda, ResourceExpression source)
        {
            this.successfulRebind = true;
            this.oldLambdaParameter = lambda.Parameters[0];
            this.projectionSource = source;

            Expression body = this.Visit(lambda.Body);
            if (this.successfulRebind)
            {
                Type delegateType = typeof(Func<,>).MakeGenericType(new Type[] { newLambdaParameter.Type, lambda.Body.Type });
#if ASTORIA_LIGHT
                return ExpressionHelpers.CreateLambda(delegateType, body, new ParameterExpression[] { this.newLambdaParameter });
#else
                return Expression.Lambda(delegateType, body, new ParameterExpression[] { this.newLambdaParameter });
#endif
            }
            else
            {
                throw new NotSupportedException(Strings.ALinq_CanOnlyProjectTheLeaf);
            }
        }

        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression == this.oldLambdaParameter)
            {
                // Member is only a valid projection target if it is the target of the current scope
                QueryableResourceExpression resourceExpression = this.projectionSource as QueryableResourceExpression;
                if (resourceExpression != null && resourceExpression.HasTransparentScope && resourceExpression.TransparentScope.Accessor == m.Member.Name)
                {
                    Debug.Assert(m.Type == this.newLambdaParameter.Type, "Should not be rewriting a parameter with a different type than the original");
                    return this.newLambdaParameter;
                }
                else
                {
                    this.successfulRebind = false;
                }
            }

            return base.VisitMemberAccess(m);
        }

        #endregion Internal methods.
    }
}
