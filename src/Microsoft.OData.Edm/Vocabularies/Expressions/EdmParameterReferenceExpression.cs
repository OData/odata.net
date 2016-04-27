//---------------------------------------------------------------------
// <copyright file="EdmParameterReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM parameter reference expression.
    /// </summary>
    public class EdmParameterReferenceExpression : EdmElement, IEdmParameterReferenceExpression
    {
        private readonly IEdmOperationParameter referencedParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmParameterReferenceExpression"/> class.
        /// </summary>
        /// <param name="referencedParameter">Referenced parameter</param>
        public EdmParameterReferenceExpression(IEdmOperationParameter referencedParameter)
        {
            EdmUtil.CheckArgumentNull(referencedParameter, "referencedParameter");
            this.referencedParameter = referencedParameter;
        }

        /// <summary>
        /// Gets the referenced parameter.
        /// </summary>
        public IEdmOperationParameter ReferencedParameter
        {
            get { return this.referencedParameter; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.ParameterReference; }
        }
    }
}
