//   OData .NET Libraries ver. 5.6.3
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

namespace System.Data.Services.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client.Metadata;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    
    #endregion Namespaces

    /// <summary>
    /// Holds state (Path, lambda parameter stack, etc) for projection analysis.
    /// </summary>
    internal class PathBox
    {
        #region Private fields

        /// <summary>This class is used as a marker for an entity projected in its entirety.</summary>
        private const char EntireEntityMarker = UriHelper.ASTERISK;

        private readonly List<StringBuilder> projectionPaths = new List<StringBuilder>();

        private readonly List<StringBuilder> expandPaths = new List<StringBuilder>();

        private readonly Stack<ParameterExpression> parameterExpressions = new Stack<ParameterExpression>();

        private readonly Dictionary<ParameterExpression, string> basePaths = new Dictionary<ParameterExpression, string>(ReferenceEqualityComparer<ParameterExpression>.Instance);

        /// <summary>The request data service version for the projection and expand paths</summary>
        private Version uriVersion;

        #endregion Private fields

        /// <summary>Initializes a new <see cref="PathBox"/> instance.</summary>
        internal PathBox()
        {
            // add a default empty path.
            projectionPaths.Add(new StringBuilder());
            this.uriVersion = Util.DataServiceVersion1;
        }

        internal IEnumerable<string> ProjectionPaths
        {
            get
            {
                return projectionPaths.Where(s => s.Length > 0).Select(s => s.ToString()).Distinct();
            }
        }

        internal IEnumerable<string> ExpandPaths
        {
            get
            {
                return expandPaths.Where(s => s.Length > 0).Select(s => s.ToString()).Distinct();
            }
        }

        /// <summary>The request data service version for the projection and expand paths</summary>
        internal Version UriVersion
        {
            get
            {
                return this.uriVersion;
            }
        }

        internal void PushParamExpression(ParameterExpression pe)
        {
            StringBuilder basePath = projectionPaths.Last();
            basePaths.Add(pe, basePath.ToString());
            projectionPaths.Remove(basePath);
            parameterExpressions.Push(pe);
        }

        internal void PopParamExpression()
        {
            parameterExpressions.Pop();
        }

        internal ParameterExpression ParamExpressionInScope
        {
            get
            {
                Debug.Assert(parameterExpressions.Count > 0);
                return parameterExpressions.Peek();
            }
        }

        /// <summary>Starts a new path.</summary>
        internal void StartNewPath()
        {
            Debug.Assert(this.ParamExpressionInScope != null, "this.ParamExpressionInScope != null -- should not be starting new path with no lambda parameter in scope.");

            StringBuilder sb = new StringBuilder(basePaths[this.ParamExpressionInScope]);
            RemoveEntireEntityMarkerIfPresent(sb);
            expandPaths.Add(new StringBuilder(sb.ToString()));
            AddEntireEntityMarker(sb);
            projectionPaths.Add(sb);
        }

        /// <summary>
        /// Appends the given property and source TypeAs to the projection and expand paths.
        /// </summary>
        /// <param name="pi">Navigation property</param>
        /// <param name="convertedSourceType">The TypeAs type if the source of the member access expression is a TypeAs operation. Null otherwise.</param>
        /// <param name="context">Data service context instance.</param>
        internal void AppendPropertyToPath(PropertyInfo pi, Type convertedSourceType, DataServiceContext context)
        {
            Debug.Assert(pi != null, "pi != null");

            StringBuilder sb;
            bool propertyTypeisEntityType = ClientTypeUtil.TypeOrElementTypeIsEntity(pi.PropertyType);

            string convertedSourceTypeName = (convertedSourceType == null) ? null : UriHelper.GetEntityTypeNameForUriAndValidateMaxProtocolVersion(convertedSourceType, context, ref this.uriVersion);
            if (propertyTypeisEntityType)
            {
                // an entity, so need to append to expand path also
                if (convertedSourceType != null)
                {
                    AppendToExpandPath(convertedSourceTypeName);
                }

                AppendToExpandPath(pi.Name);
            }

            sb = null;
            if (convertedSourceType != null)
            {
                AppendToProjectionPath(convertedSourceTypeName, false);
            }

            sb = AppendToProjectionPath(pi.Name, false);
            if (propertyTypeisEntityType)
            {
                AddEntireEntityMarker(sb);
            }
        }

        /// <summary>
        /// Appends a name of a property/link/type to the current projection path.
        /// </summary>
        /// <param name="name">name of the property/link/type which needs to be added to the select path.</param>
        /// <param name="replaceEntityMarkerIfPresent">if originally present in the path, replace the entity marker after appending the name</param>
        /// <returns>the string builder containing all the select paths.</returns>
        private StringBuilder AppendToProjectionPath(string name, bool replaceEntityMarkerIfPresent)
        {
            StringBuilder sb = projectionPaths.Last();
            bool entityMarkerPresent = RemoveEntireEntityMarkerIfPresent(sb);

            if (sb.Length > 0)
            {
                sb.Append(UriHelper.FORWARDSLASH);
            }

            sb.Append(name);

            if (entityMarkerPresent && replaceEntityMarkerIfPresent)
            {
                AddEntireEntityMarker(sb);
            }

            return sb;
        }

        /// <summary>
        /// Appends a name of a property/link/type to the current expand path.
        /// </summary>
        /// <param name="name">name of the property/link/type which needs to be added to the expand path.</param>
        private void AppendToExpandPath(string name)
        {
            StringBuilder sb = this.expandPaths.Last();
            if (sb.Length > 0)
            {
                sb.Append(UriHelper.FORWARDSLASH);
            }

            sb.Append(name);
        }

        /// <summary>
        /// If the path ends with the EntireEntityMarker, remove it from the path.
        /// </summary>
        /// <param name="sb">path</param>
        /// <returns>True if the EntireEntityMarker was found.</returns>
        private static bool RemoveEntireEntityMarkerIfPresent(StringBuilder sb)
        {
            bool entityMarkerPresent = false;
            if (sb.Length > 0 && sb[sb.Length - 1] == EntireEntityMarker)
            {
                sb.Remove(sb.Length - 1, 1);
                entityMarkerPresent = true;
            }

            if (sb.Length > 0 && sb[sb.Length - 1] == UriHelper.FORWARDSLASH)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return entityMarkerPresent;
        }

        /// <summary>
        /// Adds the EntireEntityMarker to the end of the path
        /// </summary>
        /// <param name="sb">path</param>
        private static void AddEntireEntityMarker(StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                sb.Append(UriHelper.FORWARDSLASH);
            }

            sb.Append(EntireEntityMarker);
        }
    }
}
