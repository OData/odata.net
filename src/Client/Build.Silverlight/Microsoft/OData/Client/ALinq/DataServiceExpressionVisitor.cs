//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
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
                case ResourceExpressionType.RootSingleResource:
                    return this.VisitQueryableResourceExpression((QueryableResourceExpression)exp);
                case ResourceExpressionType.ResourceNavigationPropertySingleton:
                    return this.VisitNavigationPropertySingletonExpression((NavigationPropertySingletonExpression)exp);
                case ResourceExpressionType.InputReference:
                    return this.VisitInputReferenceExpression((InputReferenceExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        /// <summary>
        /// QueryableResourceExpression visit method.
        /// </summary>
        /// <param name="rse">QueryableResource expression to visit</param>
        /// <returns>Visited QueryableResourceExpression expression</returns>
        internal virtual Expression VisitQueryableResourceExpression(QueryableResourceExpression rse)
        {
            Expression source = this.Visit(rse.Source);

            if (source != rse.Source)
            {
                rse = QueryableResourceExpression.CreateNavigationResourceExpression(rse.NodeType, rse.Type, source, rse.MemberExpression, rse.ResourceType, rse.ExpandPaths, rse.CountOption, rse.CustomQueryOptions, rse.Projection, rse.ResourceTypeAs, rse.UriVersion);
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
        /// based on the visited form of the <see cref="QueryableResourceExpression"/> that is referenced by
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
