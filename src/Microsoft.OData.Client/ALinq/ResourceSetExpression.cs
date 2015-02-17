//---------------------------------------------------------------------
// <copyright file="ResourceSetExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>ResourceSet Expression.</summary>
    [DebuggerDisplay("ResourceSetExpression {Source}.{MemberExpression}")]
    internal class ResourceSetExpression : QueryableResourceExpression
    {
        /// <summary>
        /// Creates a resource set expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="source">the source expression</param>
        /// <param name="memberExpression">property member name</param>
        /// <param name="resourceType">the element type of the resource</param>
        /// <param name="expandPaths">expand paths for resource set</param>
        /// <param name="countOption">count query option for the resource</param>
        /// <param name="customQueryOptions">custom query options for resource</param>
        /// <param name="projection">the projection expression</param>
        /// <param name="resourceTypeAs">TypeAs type</param>
        /// <param name="uriVersion">version of the Uri from the expand and projection paths</param>
        internal ResourceSetExpression(Type type, Expression source, Expression memberExpression, Type resourceType, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion)
            : base(type, source, memberExpression, resourceType, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion)
        {
        }

        /// <summary>
        /// Creates a resource set expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="source">the source expression</param>
        /// <param name="memberExpression">property member name</param>
        /// <param name="resourceType">the element type of the resource</param>
        /// <param name="expandPaths">expand paths for resource set</param>
        /// <param name="countOption">count query option for the resource</param>
        /// <param name="customQueryOptions">custom query options for resource</param>
        /// <param name="projection">the projection expression</param>
        /// <param name="resourceTypeAs">TypeAs type</param>
        /// <param name="uriVersion">version of the Uri from the expand and projection paths</param>
        /// <param name="operationName">name of function</param>
        /// <param name="operationParameters">parameters' names and values of function</param>
        /// <param name="isAction">action flag</param>
        internal ResourceSetExpression(Type type, Expression source, Expression memberExpression, Type resourceType, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion, string operationName, Dictionary<string, string> operationParameters, bool isAction)
            : base(type, source, memberExpression, resourceType, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion, operationName, operationParameters, isAction)
        {
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return this.source != null ? (ExpressionType)ResourceExpressionType.ResourceNavigationProperty : (ExpressionType)ResourceExpressionType.RootResourceSet; }
        }

        /// <summary>
        /// A resource set produces at most 1 result if constrained by a key predicate
        /// </summary>
        internal override bool IsSingleton
        {
            get { return this.KeyPredicateConjuncts.Count > 0; }
        }

        /// <summary>
        /// Has a key predicate restriction been applied to this ResourceSet?
        /// </summary>
        internal bool HasKeyPredicate
        {
            get { return this.KeyPredicateConjuncts.Count > 0; }
        }

        /// <summary>
        /// A resource set invocates functions
        /// </summary>
        internal override bool IsOperationInvocation
        {
            get { return this.OperationName != null; }
        }

        /// <summary>
        /// Creates a clone of the current ResourceSetExpression with the specified expression and resource types
        /// </summary>
        /// <param name="newType">The new expression type</param>
        /// <param name="newResourceType">The new resource type</param>
        /// <returns>A copy of this with the new types</returns>
        protected override QueryableResourceExpression CreateCloneWithNewTypes(Type newType, Type newResourceType)
        {
            return new ResourceSetExpression(
                newType,
                this.source,
                this.MemberExpression,
                newResourceType,
                this.ExpandPaths.ToList(),
                this.CountOption,
                this.CustomQueryOptions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                this.Projection,
                this.ResourceTypeAs,
                this.UriVersion,
                this.OperationName,
                this.OperationParameters,
                this.IsAction);
        }
    }
}
