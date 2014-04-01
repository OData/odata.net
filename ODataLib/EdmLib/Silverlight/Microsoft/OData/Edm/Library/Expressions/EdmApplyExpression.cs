//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Expressions
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
