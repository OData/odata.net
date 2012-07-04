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
