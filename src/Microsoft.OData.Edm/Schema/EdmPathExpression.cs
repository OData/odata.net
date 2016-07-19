//---------------------------------------------------------------------
// <copyright file="EdmPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM path expression.
    /// </summary>
    public class EdmPathExpression : EdmElement, IEdmPathExpression
    {
        private IEnumerable<string> pathSegments;
        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path string containing segments separated by '/'. For example: "A.B/C/D.E/Func1(NS.T,NS.T2)/P1".</param>
        public EdmPathExpression(string path)
        {
            EdmUtil.CheckArgumentNull(path, "path");
            this.path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPathExpression"/> class.
        /// </summary>
        /// <param name="pathSegments">Path segments.</param>
        public EdmPathExpression(params string[] pathSegments)
            : this((IEnumerable<string>)pathSegments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPathExpression"/> class.
        /// </summary>
        /// <param name="pathSegments">Path segments.</param>
        public EdmPathExpression(IEnumerable<string> pathSegments)
        {
            EdmUtil.CheckArgumentNull(pathSegments, "pathSegments");

            if (pathSegments.Any(segment => segment.Contains("/")))
            {
                throw new ArgumentException(Strings.PathSegmentMustNotContainSlash);
            }

            this.pathSegments = pathSegments.ToList();
        }

        /// <summary>
        /// Gets the path segments as a decomposed qualified name. "A.B/C/D.E/Func1(NS.T,NS.T2)/P1" is { "A.B", "C", "D.E", "Func1(NS.T,NS.T2)", "P1" }.
        /// </summary>
        public IEnumerable<string> PathSegments
        {
            get { return this.pathSegments ?? (this.pathSegments = this.path.Split('/')); }
        }

        /// <summary>
        /// Gets the path string, like "A.B/C/D.E".
        /// </summary>
        public string Path
        {
            get { return this.path ?? (this.path = string.Join("/", this.pathSegments.ToArray())); }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public virtual EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Path; }
        }
    }
}
