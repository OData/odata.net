//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM function reference expression.
    /// </summary>
    public class EdmFunctionReferenceExpression : EdmElement, IEdmFunctionReferenceExpression
    {
        private readonly IEdmFunction referencedFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunctionReferenceExpression"/> class.
        /// </summary>
        /// <param name="referencedFunction">Referenced function</param>
        public EdmFunctionReferenceExpression(IEdmFunction referencedFunction)
        {
            EdmUtil.CheckArgumentNull(referencedFunction, "referencedFunction");
            this.referencedFunction = referencedFunction;
        }

        /// <summary>
        /// Gets the referenced function.
        /// </summary>
        public IEdmFunction ReferencedFunction
        {
            get { return this.referencedFunction; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.FunctionReference; }
        }
    }
}
