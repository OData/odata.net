//---------------------------------------------------------------------
// <copyright file="EdmApplyExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM operation application expression.
    /// </summary>
    public class EdmApplyExpression : EdmElement, IEdmApplyExpression
    {
        private readonly IEdmExpression appliedOperation;
        private readonly IEnumerable<IEdmExpression> arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmApplyExpression"/> class.
        /// </summary>
        /// <param name="appliedOperation">Operation to apply.</param>
        /// <param name="arguments">Application arguments. Value may be null, in which case it is treated as an empty enumerable.</param>
        public EdmApplyExpression(IEdmOperation appliedOperation, params IEdmExpression[] arguments)
            : this(appliedOperation, (IEnumerable<IEdmExpression>)arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmApplyExpression"/> class.
        /// </summary>
        /// <param name="appliedOperation">Operation to apply.</param>
        /// <param name="arguments">Application arguments. Value may be null, in which case it is treated as an empty enumerable.</param>
        public EdmApplyExpression(IEdmOperation appliedOperation, IEnumerable<IEdmExpression> arguments)
            : this(new EdmOperationReferenceExpression(EdmUtil.CheckArgumentNull(appliedOperation, "appliedFunction")), arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmApplyExpression"/> class.
        /// </summary>
        /// <param name="appliedOperation">Operation to apply.</param>
        /// <param name="arguments">Application arguments. Value may be null, in which case it is treated as an empty enumerable.</param>
        public EdmApplyExpression(IEdmExpression appliedOperation, IEnumerable<IEdmExpression> arguments)
        {
            EdmUtil.CheckArgumentNull(appliedOperation, "appliedFunction");
            EdmUtil.CheckArgumentNull(arguments, "arguments");

            this.appliedOperation = appliedOperation;
            this.arguments = arguments;
        }

        /// <summary>
        /// Gets the applied operation.
        /// </summary>
        public IEdmExpression AppliedOperation
        {
            get { return this.appliedOperation; }
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
            get { return EdmExpressionKind.OperationApplication; }
        }
    }
}
