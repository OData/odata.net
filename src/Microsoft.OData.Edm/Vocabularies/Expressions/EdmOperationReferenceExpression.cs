//---------------------------------------------------------------------
// <copyright file="EdmOperationReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Vocabularies
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
