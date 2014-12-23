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
    using System.Diagnostics;
    using System.Linq.Expressions;
    using Microsoft.OData.Client.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Use this class to represent a step in a path of segments
    /// over a parsed tree used during projection-driven materialization.
    /// </summary>
    [DebuggerDisplay("Segment {ProjectionType} {Member}")]
    internal class ProjectionPathSegment
    {
        #region Constructors

        /// <summary>Initializes a new <see cref="ProjectionPathSegment"/> instance.</summary>
        /// <param name="startPath">Path on which this segment is located.</param>
        /// <param name="member">Name of member to access when traversing a property; possibly null.</param>
        /// <param name="projectionType">
        /// Type that we expect to project out; typically the same as <paramref name="member"/>, but may be adjusted.
        /// </param>
        internal ProjectionPathSegment(ProjectionPath startPath, string member, Type projectionType)
        {
            Debug.Assert(startPath != null, "startPath != null");
            
            this.Member = member;
            this.StartPath = startPath;
            this.ProjectionType = projectionType;
        }

        /// <summary>Initializes a new <see cref="ProjectionPathSegment"/> instance.</summary>
        /// <param name="startPath">Path on which this segment is located.</param>
        /// <param name="memberExpression">Member expression for the projection path; possibly null.</param>
        internal ProjectionPathSegment(ProjectionPath startPath, MemberExpression memberExpression)
        {
            Debug.Assert(startPath != null, "startPath != null");
            Debug.Assert(memberExpression != null, "memberExpression != null");

            this.StartPath = startPath;

            Expression source = ResourceBinder.StripTo<Expression>(memberExpression.Expression);
            this.Member = ClientTypeUtil.GetServerDefinedName(memberExpression.Member);
            this.ProjectionType = memberExpression.Type;
            this.SourceTypeAs = source.NodeType == ExpressionType.TypeAs ? source.Type : null;
        }

        #endregion Constructors

        #region Internal properties

        /// <summary>Name of member to access when traversing a property; possibly null.</summary>
        internal string Member 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Type that we expect to project out; typically the same as <propertyref name="Member"/>, but may be adjusted.
        /// </summary>
        /// <remarks>
        /// In particular, this type will be adjusted for nested narrowing entity types.
        /// 
        /// For example:
        /// from c in ctx.Customers select new NarrowCustomer() { 
        ///   ID = c.ID, 
        ///   BestFriend = new NarrowCustomer() { ID = c.BestFriend.ID }
        /// }
        /// 
        /// In this case, ID will match types on both sides, but BestFriend
        /// will be of type Customer in the member access of the source tree
        /// and we want to project out a member-initialized NarrowCustomer
        /// in the target tree.
        /// </remarks>
        internal Type ProjectionType 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Contains the TypeAs at the source of the member access, null otherwise
        /// e.g. (p as Employee).Manager
        /// </summary>
        internal Type SourceTypeAs
        {
            get;
            set;
        }

        /// <summary>Path on which this segment is located.</summary>
        internal ProjectionPath StartPath 
        { 
            get; 
            private set; 
        }

        #endregion Internal properties
    }
}
