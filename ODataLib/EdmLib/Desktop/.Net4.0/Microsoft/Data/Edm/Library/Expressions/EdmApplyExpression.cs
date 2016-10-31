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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM function application expression.
    /// </summary>
    public class EdmApplyExpression : EdmElement, IEdmApplyExpression
    {
        private readonly IEdmExpression appliedFunction;
        private readonly IEnumerable<IEdmExpression> arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmApplyExpression"/> class.
        /// </summary>
        /// <param name="appliedFunction">Function to apply.</param>
        /// <param name="arguments">Application arguments. Value may be null, in which case it is treated as an empty enumerable.</param>
        public EdmApplyExpression(IEdmFunction appliedFunction, params IEdmExpression[] arguments)
            : this(appliedFunction, (IEnumerable<IEdmExpression>)arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmApplyExpression"/> class.
        /// </summary>
        /// <param name="appliedFunction">Function to apply.</param>
        /// <param name="arguments">Application arguments. Value may be null, in which case it is treated as an empty enumerable.</param>
        public EdmApplyExpression(IEdmFunction appliedFunction, IEnumerable<IEdmExpression> arguments)
            : this(new EdmFunctionReferenceExpression(EdmUtil.CheckArgumentNull(appliedFunction, "appliedFunction")), arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmApplyExpression"/> class.
        /// </summary>
        /// <param name="appliedFunction">Function to apply.</param>
        /// <param name="arguments">Application arguments. Value may be null, in which case it is treated as an empty enumerable.</param>
        public EdmApplyExpression(IEdmExpression appliedFunction, IEnumerable<IEdmExpression> arguments)
        {
            EdmUtil.CheckArgumentNull(appliedFunction, "appliedFunction");
            EdmUtil.CheckArgumentNull(arguments, "arguments");

            this.appliedFunction = appliedFunction;
            this.arguments = arguments;
        }

        /// <summary>
        /// Gets the applied function.
        /// </summary>
        public IEdmExpression AppliedFunction
        {
            get { return this.appliedFunction; }
        }

        /// <summary>
        /// Gets the arguments to the function.
        /// </summary>
        public IEnumerable<IEdmExpression> Arguments
        {
            get { return this.arguments; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.FunctionApplication; }
        }
    }
}
