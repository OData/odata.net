//---------------------------------------------------------------------
// <copyright file="EdmEnumMemberReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM enumeration member reference expression.
    /// </summary>
    public class EdmEnumMemberReferenceExpression : EdmElement, IEdmEnumMemberReferenceExpression
    {
        private readonly IEdmEnumMember referencedEnumMember;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumMemberReferenceExpression"/> class.
        /// </summary>
        /// <param name="referencedEnumMember">Referenced enum member.</param>
        public EdmEnumMemberReferenceExpression(IEdmEnumMember referencedEnumMember)
        {
            EdmUtil.CheckArgumentNull(referencedEnumMember, "referencedEnumMember");
            this.referencedEnumMember = referencedEnumMember;
        }

        /// <summary>
        /// Gets the referenced enum member.
        /// </summary>
        public IEdmEnumMember ReferencedEnumMember
        {
            get { return this.referencedEnumMember; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EnumMemberReference; }
        }
    }
}
