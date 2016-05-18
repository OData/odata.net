//---------------------------------------------------------------------
// <copyright file="SelectExpandPathBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Client.ALinq.UriParser;
    using Microsoft.OData.Client.Metadata;


    #endregion Namespaces

    /// <summary>
    /// Holds state (Path, lambda parameter stack, etc) for projection analysis.
    /// </summary>
    internal class SelectExpandPathBuilder
    {
        #region Private fields

        /// <summary>
        /// This class is used as a marker for an entity projected in its entirety.
        /// </summary>
        private const char EntireEntityMarker = UriHelper.ASTERISK;

        /// <summary>
        /// The list of projection paths as PathSegmentTokens.
        /// </summary>
        private readonly List<PathSegmentToken> projectionPaths = new List<PathSegmentToken>();

        /// <summary>
        /// The list of expansion paths as PathSegmentTokens.
        /// </summary>
        private readonly List<PathSegmentToken> expandPaths = new List<PathSegmentToken>();

        /// <summary>
        /// The stack of parameter expressions.
        /// </summary>
        private readonly Stack<ParameterExpression> parameterExpressions = new Stack<ParameterExpression>();

        /// <summary>
        /// The dictionary linking parameter expressions to their base paths.
        /// </summary>
        private readonly Dictionary<ParameterExpression, PathSegmentToken> basePaths = new Dictionary<ParameterExpression, PathSegmentToken>(ReferenceEqualityComparer<ParameterExpression>.Instance);

        /// <summary>
        /// The request data service version for the projection and expand paths
        /// </summary>
        private Version uriVersion;

        /// <summary>
        /// Flag indicating whether we're processing the first segment in a new path.
        /// </summary>
        private bool firstSegmentInNewPath;

        /// <summary>
        /// Summary to indicate whether we're building from an empty base path or not
        /// </summary>
        private bool basePathIsEmpty;

        #endregion Private fields

        /// <summary>
        /// Initializes a new <see cref="SelectExpandPathBuilder"/> instance.
        /// </summary>
        public SelectExpandPathBuilder()
        {
            firstSegmentInNewPath = true;
            this.uriVersion = Util.ODataVersion4;
        }

        /// <summary>
        /// Get a list of strings that represent the current list of ProjectionPaths
        /// </summary>
        public IEnumerable<string> ProjectionPaths
        {
            get
            {
                return this.WriteProjectionPaths();
            }
        }

        /// <summary>
        /// Get a list of strings that represent the current list of ExpansionPaths.
        /// </summary>
        public IEnumerable<string> ExpandPaths
        {
            get
            {
                return this.WriteExpansionPaths();
            }
        }

        /// <summary>
        /// The request data service version for the projection and expand paths
        /// </summary>
        public Version UriVersion
        {
            get
            {
                return this.uriVersion;
            }
        }

        /// <summary>
        /// Get the parameter expression that is currently in scope.
        /// </summary>
        public ParameterExpression ParamExpressionInScope
        {
            get
            {
                Debug.Assert(parameterExpressions.Count > 0, "parameterExpressions.Count > 0");
                return parameterExpressions.Peek();
            }
        }

        /// <summary>
        /// Add a new ParameterExpression to the stack.
        /// </summary>
        /// <param name="pe">The parameter expression to add.</param>
        public void PushParamExpression(ParameterExpression pe)
        {
            PathSegmentToken basePath = expandPaths.LastOrDefault();
            basePaths.Add(pe, basePath);
            expandPaths.Remove(basePath);
            parameterExpressions.Push(pe);
        }

        /// <summary>
        /// Pop the top parameter expression off the stack.
        /// </summary>
        public void PopParamExpression()
        {
            parameterExpressions.Pop();
        }

        /// <summary>
        /// Starts a new path.
        /// </summary>
        public void StartNewPath()
        {
            Debug.Assert(this.ParamExpressionInScope != null, "this.ParamExpressionInScope != null -- should not be starting new path with no lambda parameter in scope.");

            PathSegmentToken basePath = basePaths[this.ParamExpressionInScope];
            PathSegmentToken newExpandPathToAdd;
            if (basePath != null)
            {
                NewTreeBuilder newTreeBuilder = new NewTreeBuilder();
                newExpandPathToAdd = basePath.Accept(newTreeBuilder);
            }
            else
            {
                newExpandPathToAdd = null;
            }

            expandPaths.Add(newExpandPathToAdd);

            firstSegmentInNewPath = true;
            basePathIsEmpty = basePath == null;
        }

        /// <summary>
        /// Appends the given property and source TypeAs to the projection and expand paths.
        /// </summary>
        /// <param name="pi">Navigation property</param>
        /// <param name="convertedSourceType">The TypeAs type if the source of the member access expression is a TypeAs operation. Null otherwise.</param>
        /// <param name="context">Data service context instance.</param>
        public void AppendPropertyToPath(PropertyInfo pi, Type convertedSourceType, DataServiceContext context)
        {
            Debug.Assert(pi != null, "pi != null");

            bool propertyTypeisEntityType = ClientTypeUtil.TypeOrElementTypeIsEntity(pi.PropertyType);

            string convertedSourceTypeName = (convertedSourceType == null) ?
                null :
                UriHelper.GetEntityTypeNameForUriAndValidateMaxProtocolVersion(convertedSourceType, context, ref this.uriVersion);

            string propertyServerDefinedName = ClientTypeUtil.GetServerDefinedName(pi);

            string propertyName = convertedSourceType != null ?
                String.Join(UriHelper.FORWARDSLASH.ToString(), new string[] { convertedSourceTypeName, propertyServerDefinedName }) :
                propertyServerDefinedName;

            if (propertyTypeisEntityType)
            {
                // an entity, so need to append to expand path only
                AppendToExpandPath(propertyName, false);
            }
            else
            {
                // if this is a non-entity, then it an either be
                // 1) a top level property that we're selecting
                //          -or-
                // 2) a lower level property being selected via a nav prop.
                //
                // if 1) Then we just add it to the projection path
                // if 2) then we add it to the expand path instead.

                // we decide that based on whether this is the first property we're adding to
                // the path, and whether the base path we're starting from is empty.
                if (firstSegmentInNewPath && basePathIsEmpty)
                {
                    AppendToProjectionPath(propertyName);
                }
                else
                {
                    AppendToExpandPath(propertyName, true);
                }
            }

            // clear the firstSegmentInPath flag
            firstSegmentInNewPath = false;
        }

        /// <summary>
        /// Write out the current list of expansion paths as a list of strings.
        /// </summary>
        /// <returns>The current list of expansion paths as a list of strings.</returns>
        private IEnumerable<string> WriteExpansionPaths()
        {
            SelectExpandPathToStringVisitor visitor = new SelectExpandPathToStringVisitor();
            return this.expandPaths.Where(path => path != null).Select(path => path.Accept(visitor));
        }

        /// <summary>
        /// Write out the current list of projection paths as a list of strings.
        /// </summary>
        /// <returns>The current list of projection paths as a list of strings.</returns>
        private IEnumerable<string> WriteProjectionPaths()
        {
            return this.projectionPaths.Where(path => path != null).Select(path => path.Identifier);
        }

        /// <summary>
        /// Appends a name of a property/link/type to the current projection path.
        /// </summary>
        /// <param name="name">name of the property/link/type which needs to be added to the select path.</param>
        private void AppendToProjectionPath(string name)
        {
            // get rid of the * if its present
            foreach (PathSegmentToken pathSegment in projectionPaths)
            {
                if (pathSegment != null && pathSegment.Identifier == EntireEntityMarker.ToString())
                {
                    projectionPaths.Remove(pathSegment);
                }
            }

            projectionPaths.Add(new NonSystemToken(name, /*namedValues*/ null, /*nextToken*/ null));
        }

        /// <summary>
        /// Appends a name of a property/link/type to the current expand path.
        /// </summary>
        /// <param name="name">name of the property/link/type which needs to be added to the expand path.</param>
        /// <param name="isStructural">is this a structural property.</param>
        private void AppendToExpandPath(string name, bool isStructural)
        {
            PathSegmentToken path = this.expandPaths.LastOrDefault();
            NonSystemToken newToken = new NonSystemToken(name, /*namedValues*/null, /*nextToken*/null);
            newToken.IsStructuralProperty = isStructural;
            if (path != null)
            {
                expandPaths.Remove(path);
                AddNewEndingTokenVisitor addNewEndingTokenVisitor = new AddNewEndingTokenVisitor(newToken);
                path.Accept(addNewEndingTokenVisitor);
                expandPaths.Add(path);
            }
            else
            {
                expandPaths.Add(newToken);
            }
        }
    }
}
