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
        private IEnumerable<string> path;
        private string fullPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path string containing segments separated by '/'. For example: "A.B/C/D.E/Func1(NS.T,NS.T2)/P1".</param>
        public EdmPathExpression(string path)
        {
            EdmUtil.CheckArgumentNull(path, "path");
            this.fullPath = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path segments.</param>
        public EdmPathExpression(params string[] path)
            : this((IEnumerable<string>)path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path segments.</param>
        public EdmPathExpression(IEnumerable<string> path)
        {
            EdmUtil.CheckArgumentNull(path, "path");

            if (path.Any(segment => segment.Contains("/")))
            {
                throw new ArgumentException(Strings.PathSegmentMustNotContainSlash);
            }

            this.path = path;
        }

        /// <summary>
        /// Gets the path as a decomposed qualified name. "A.B/C/D.E/Func1(NS.T,NS.T2)/P1" is { "A.B", "C", "D.E", "Func1(NS.T,NS.T2)", "P1" }.
        /// </summary>
        public IEnumerable<string> Path
        {
            get { return this.path ?? (this.path = this.fullPath.Split('/')); }
        }

        /// <summary>
        /// Gets the full path string, like "A.B/C/D.E".
        /// </summary>
        public string FullPath
        {
            get { return this.fullPath ?? (this.fullPath = string.Join("/", this.path.ToArray())); }
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
