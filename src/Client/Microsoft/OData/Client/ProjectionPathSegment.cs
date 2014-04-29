//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
