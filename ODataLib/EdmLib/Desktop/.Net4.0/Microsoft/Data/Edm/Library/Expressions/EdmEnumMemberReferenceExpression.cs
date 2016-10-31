//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
