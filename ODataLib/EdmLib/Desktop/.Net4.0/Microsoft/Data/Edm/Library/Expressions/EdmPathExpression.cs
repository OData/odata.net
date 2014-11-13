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

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Expressions
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
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Path; }
        }
    }
}
