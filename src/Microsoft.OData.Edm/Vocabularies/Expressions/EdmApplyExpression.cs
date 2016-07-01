//---------------------------------------------------------------------
// <copyright file="EdmApplyExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM operation application expression.
    /// </summary>
    public class EdmApplyExpression : EdmElement, IEdmApplyExpression
    {
        private readonly IEdmFunction appliedFunction;
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
        {
            EdmUtil.CheckArgumentNull(appliedFunction, "appliedFunction");
            EdmUtil.CheckArgumentNull(arguments, "arguments");

            this.appliedFunction = appliedFunction;
            this.arguments = arguments;
        }

        /// <summary>
        /// Gets the applied function.
        /// </summary>
        public IEdmFunction AppliedFunction
        {
            get { return this.appliedFunction; }
        }

        /// <summary>
        /// Gets the arguments to the operation.
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
