//---------------------------------------------------------------------
// <copyright file="SingletonResourceExpression.cs" company="Microsoft">
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

    /// <summary>Singleton Resource Expression, used to represent singleton, may extend to support FunctionImport</summary>
    [DebuggerDisplay("SingletonResourceExpression {Source}.{MemberExpression}")]
    internal class SingletonResourceExpression : QueryableResourceExpression
    {
        /// <summary>
        /// Creates a singleton resource expression
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
        internal SingletonResourceExpression(Type type, Expression source, Expression memberExpression, Type resourceType, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion) :
            base(type, source, memberExpression, resourceType, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion)
        {
            UseFilterAsPredicate = true;
        }

        /// <summary>
        /// Creates a singleton resource expression
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
        /// <param name="functionName">name of function</param>
        /// <param name="functionParameters">parameters' names and values of function</param>
        /// <param name="isAction">action flag</param>
        internal SingletonResourceExpression(Type type, Expression source, Expression memberExpression, Type resourceType, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion, string functionName, Dictionary<string, string> functionParameters, bool isAction) :
            base(type, source, memberExpression, resourceType, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion, functionName, functionParameters, isAction)
        {
            UseFilterAsPredicate = true;
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)ResourceExpressionType.RootSingleResource; }
        }

        /// <summary>
        /// Always be singleton.
        /// </summary>
        internal override bool IsSingleton
        {
            get { return true; }
        }

        /// <summary>
        /// Maybe function import invocation.
        /// </summary>
        internal override bool IsOperationInvocation
        {
            get { return this.OperationName != null; }
        }

        /// <summary>
        /// Creates a clone of the current SingletResourceExpression with the specified expression and resource types
        /// </summary>
        /// <param name="newType">The new expression type</param>
        /// <param name="newResourceType">The new resource type</param>
        /// <returns>A copy of this with the new types</returns>
        protected override QueryableResourceExpression CreateCloneWithNewTypes(Type newType, Type newResourceType)
        {
            return new SingletonResourceExpression(
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
