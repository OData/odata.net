//---------------------------------------------------------------------
// <copyright file="EdmPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM path expression.
    /// </summary>
    public class EdmPathExpression : EdmElement, IEdmPathExpression
    {
        private readonly IEnumerable<string> path;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path string containing segments seperated by '/'. For example: "A.B/C/D.E/Func1(NS.T,NS.T2)/P1".</param>
        public EdmPathExpression(string path)
            : this(EdmUtil.CheckArgumentNull(path, "path").Split('/'))
        {
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

            foreach (string segment in path)
            {
                if (segment.Contains("/"))
                {
                    throw new ArgumentException(Strings.PathSegmentMustNotContainSlash);
                }
            }

            this.path = path;
        }

        /// <summary>
        /// Gets the path as a decomposed qualified name. "A.B/C/D.E/Func1(NS.T,NS.T2)/P1" is { "A.B", "C", "D.E", "Func1(NS.T,NS.T2)", "P1" }.
        /// </summary>
        public IEnumerable<string> Path
        {
            get { return this.path; }
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
