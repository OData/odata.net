//   OData .NET Libraries ver. 6.8.1
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

using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM type test expression.
    /// </summary>
    public class EdmIsTypeExpression : EdmElement, IEdmIsTypeExpression
    {
        private readonly IEdmExpression operand;
        private readonly IEdmTypeReference type;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmIsTypeExpression"/> class.
        /// </summary>
        /// <param name="operand">Expression whose type is to be tested.</param>
        /// <param name="type">Type to test.</param>
        public EdmIsTypeExpression(IEdmExpression operand, IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(operand, "operand");
            EdmUtil.CheckArgumentNull(type, "type");

            this.operand = operand;
            this.type = type;
        }

        /// <summary>
        /// Gets the expression whose type is to be tested.
        /// </summary>
        public IEdmExpression Operand
        {
            get { return this.operand; }
        }

        /// <summary>
        /// Gets the type to be tested against.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.IsType; }
        }
    }
}
