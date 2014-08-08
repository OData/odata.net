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

namespace System.Data.Services.Client
{
    #region Namespaces

    using System;
    using System.Data.Services.Client.Metadata;
    using System.Data.Services.Common;
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
                ResourceSetExpression resourceSetExpression = this.projectionSource as ResourceSetExpression;
                if (resourceSetExpression != null && resourceSetExpression.HasTransparentScope && resourceSetExpression.TransparentScope.Accessor == m.Member.Name)
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
