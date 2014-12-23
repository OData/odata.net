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
    #region Private fields

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    #endregion Private fields

    /// <summary>Expression for a navigation property into a single entity (eg: Customer.BestFriend).</summary>
    internal class NavigationPropertySingletonExpression : ResourceExpression
    {
        #region Private fields

        /// <summary>property member name</summary>
        private readonly Expression memberExpression;

        /// <summary> resource type</summary>
        private readonly Type resourceType;

        #endregion Private fields

        /// <summary>
        /// Creates a NavigationPropertySingletonExpression expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="source">the source expression</param>
        /// <param name="memberExpression">property member name</param>
        /// <param name="resourceType">resource type for expression</param>
        /// <param name="expandPaths">expand paths for resource set</param>
        /// <param name="countOption">count option for the resource set</param>
        /// <param name="customQueryOptions">custom query options for resourcse set</param>
        /// <param name="projection">projection expression</param>
        /// <param name="resourceTypeAs">target expression type for a TypeAs conversion</param>
        /// <param name="uriVersion">version of the Uri from the expand and projection paths</param>
        internal NavigationPropertySingletonExpression(Type type, Expression source, Expression memberExpression, Type resourceType, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion)
            : base(source, type, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion)
        {
            Debug.Assert(memberExpression != null, "memberExpression != null");
            Debug.Assert(resourceType != null, "resourceType != null");

            this.memberExpression = memberExpression;
            this.resourceType = resourceType;
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)ResourceExpressionType.ResourceNavigationPropertySingleton; }
        }

        /// <summary>
        /// Gets the member expression.
        /// </summary>
        internal MemberExpression MemberExpression
        {
            get
            {
                return (MemberExpression)this.memberExpression;
            }
        }

        /// <summary>
        /// The resource type of the singe instance produced by this singleton navigation.
        /// </summary>
        internal override Type ResourceType
        {
            get { return this.resourceType; }
        }

        /// <summary>
        /// Singleton navigation properties always produce at most 1 result
        /// </summary>
        internal override bool IsSingleton
        {
            get { return true; }
        }

        /// <summary>
        /// Does Singleton navigation have query options.
        /// </summary>
        internal override bool HasQueryOptions
        {
            get
            {
                return this.ExpandPaths.Count > 0 ||
                    this.CountOption == CountOption.CountQuery ||
                    this.CustomQueryOptions.Count > 0 ||
                    this.Projection != null;
            }
        }

        /// <summary>
        /// Whether this is a function invocation.
        /// </summary>
        internal override bool IsOperationInvocation
        {
            get { return false; }
        }

        /// <summary>
        /// Cast changes the type of the ResourceExpression
        /// </summary>
        /// <param name="type">new type</param>
        /// <returns>new NavigationPropertySingletonExpression</returns>
        internal override ResourceExpression CreateCloneWithNewType(Type type)
        {
            return new NavigationPropertySingletonExpression(
                type,
                this.source,
                this.MemberExpression,
                TypeSystem.GetElementType(type),
                this.ExpandPaths.ToList(),
                this.CountOption,
                this.CustomQueryOptions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                this.Projection,
                this.ResourceTypeAs,
                this.UriVersion);
        }
    }
}
