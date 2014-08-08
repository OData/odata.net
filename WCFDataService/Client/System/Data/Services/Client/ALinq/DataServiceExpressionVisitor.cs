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

    using System.Diagnostics;
    using System.Linq.Expressions;

    #endregion Namespaces

    /// <summary>
    /// Specific Vistior base class for the DataServiceQueryProvider.  
    /// </summary>
    internal abstract class DataServiceALinqExpressionVisitor : ALinqExpressionVisitor
    {
        /// <summary>
        /// Main visit method.
        /// </summary>
        /// <param name="exp">Expression to visit</param>
        /// <returns>Visited expression</returns>
        internal override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }

            switch ((ResourceExpressionType)exp.NodeType)
            {
                case ResourceExpressionType.RootResourceSet:
                case ResourceExpressionType.ResourceNavigationProperty:
                    return this.VisitResourceSetExpression((ResourceSetExpression)exp);
                case ResourceExpressionType.ResourceNavigationPropertySingleton:
                    return this.VisitNavigationPropertySingletonExpression((NavigationPropertySingletonExpression)exp);
                case ResourceExpressionType.InputReference:
                    return this.VisitInputReferenceExpression((InputReferenceExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        /// <summary>
        /// ResourceSetExpression visit method.
        /// </summary>
        /// <param name="rse">ResourceSetExpression expression to visit</param>
        /// <returns>Visited ResourceSetExpression expression</returns>
        internal virtual Expression VisitResourceSetExpression(ResourceSetExpression rse)
        {
            Expression source = this.Visit(rse.Source);

            if (source != rse.Source)
            {
                rse = new ResourceSetExpression(rse.Type, source, rse.MemberExpression, rse.ResourceType, rse.ExpandPaths, rse.CountOption, rse.CustomQueryOptions, rse.Projection, rse.ResourceTypeAs, rse.UriVersion);
            }

            return rse;
        }

        /// <summary>
        /// NavigationPropertySingletonExpressionvisit method.
        /// </summary>
        /// <param name="npse">NavigationPropertySingletonExpression expression to visit</param>
        /// <returns>Visited NavigationPropertySingletonExpression expression</returns>
        internal virtual Expression VisitNavigationPropertySingletonExpression(NavigationPropertySingletonExpression npse)
        {
            Expression source = this.Visit(npse.Source);

            if (source != npse.Source)
            {
                npse = new NavigationPropertySingletonExpression(npse.Type, source, npse.MemberExpression, npse.MemberExpression.Type, npse.ExpandPaths, npse.CountOption, npse.CustomQueryOptions, npse.Projection, npse.ResourceTypeAs, npse.UriVersion);
            }

            return npse;
        }

        /// <summary>
        /// Visit an <see cref="InputReferenceExpression"/>, producing a new InputReferenceExpression
        /// based on the visited form of the <see cref="ResourceSetExpression"/> that is referenced by
        /// the InputReferenceExpression argument, <paramref name="ire"/>.
        /// </summary>
        /// <param name="ire">InputReferenceExpression expression to visit</param>
        /// <returns>Visited InputReferenceExpression expression</returns>
        internal virtual Expression VisitInputReferenceExpression(InputReferenceExpression ire)
        {
            Debug.Assert(ire != null, "ire != null -- otherwise caller never should have visited here");
            ResourceExpression re = (ResourceExpression)this.Visit(ire.Target);
            return re.CreateReference();
        }
    }
}
