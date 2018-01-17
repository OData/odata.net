//---------------------------------------------------------------------
// <copyright file="EdmEnumMemberExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM enumeration member reference expression.
    /// </summary>
    public class EdmEnumMemberExpression : EdmElement, IEdmEnumMemberExpression
    {
        private readonly List<IEdmEnumMember> enumMembers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumMemberExpression"/> class.
        /// </summary>
        /// <param name="enumMembers">Referenced enum member.</param>
        public EdmEnumMemberExpression(params IEdmEnumMember[] enumMembers)
        {
            EdmUtil.CheckArgumentNull(enumMembers, "referencedEnumMember");
            Debug.Assert(enumMembers.Any(), "enumMembers is empty.");
            this.enumMembers = enumMembers.ToList();
        }

        /// <summary>
        /// Gets the referenced enum member.
        /// </summary>
        public IEnumerable<IEdmEnumMember> EnumMembers
        {
            get { return this.enumMembers; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EnumMember; }
        }
    }
}
