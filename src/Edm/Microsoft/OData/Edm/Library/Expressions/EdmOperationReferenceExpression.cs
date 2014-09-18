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
