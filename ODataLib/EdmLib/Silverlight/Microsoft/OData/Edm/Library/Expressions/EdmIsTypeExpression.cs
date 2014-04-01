//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
