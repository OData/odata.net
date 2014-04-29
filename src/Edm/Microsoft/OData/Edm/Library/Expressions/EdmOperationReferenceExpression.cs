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
    /// Represents an EDM operation reference expression.
    /// </summary>
    public class EdmOperationReferenceExpression : EdmElement, IEdmOperationReferenceExpression
    {
        private readonly IEdmOperation referencedOperation;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationReferenceExpression"/> class.
        /// </summary>
        /// <param name="referencedOperation">Referenced operation</param>
        public EdmOperationReferenceExpression(IEdmOperation referencedOperation)
        {
            EdmUtil.CheckArgumentNull(referencedOperation, "referencedFunction");
            this.referencedOperation = referencedOperation;
        }

        /// <summary>
        /// Gets the referenced operation.
        /// </summary>
        public IEdmOperation ReferencedOperation
        {
            get { return this.referencedOperation; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.OperationReference; }
        }
    }
}
