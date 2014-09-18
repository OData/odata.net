//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM property path expression.
    /// </summary>
    public class EdmPropertyPathExpression : EdmPathExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path string containing segments seperated by '/'. For example: "A.B/C/D.E/Func1(NS.T,NS.T2)/P1".</param>
        public EdmPropertyPathExpression(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path segments.</param>
        public EdmPropertyPathExpression(params string[] path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path segments.</param>
        public EdmPropertyPathExpression(IEnumerable<string> path) 
            : base(path)
        {
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.PropertyPath; }
        }
    }
}
