//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Expressions
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
