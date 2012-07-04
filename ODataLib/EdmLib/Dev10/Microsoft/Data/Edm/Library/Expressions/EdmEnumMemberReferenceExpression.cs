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

using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Expressions
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
