//---------------------------------------------------------------------
// <copyright file="EdmEnumMember.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a member of an EDM enumeration type.
    /// </summary>
    public class EdmEnumMember : EdmNamedElement, IEdmEnumMember
    {
        private readonly IEdmEnumType declaringType;
        private IEdmEnumMemberValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumMember"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this member.</param>
        /// <param name="name">Name of this enumeration member.</param>
        /// <param name="value">Value of this enumeration member.</param>
        public EdmEnumMember(IEdmEnumType declaringType, string name, IEdmEnumMemberValue value)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringType, "declaringType");
            EdmUtil.CheckArgumentNull(value, "value");

            this.declaringType = declaringType;
            this.value = value;
        }

        /// <summary>
        /// Gets the type that this member belongs to.
        /// </summary>
        public IEdmEnumType DeclaringType
        {
            get { return this.declaringType; }
        }

        /// <summary>
        /// Gets the value of this enumeration type member.
        /// </summary>
        public IEdmEnumMemberValue Value
        {
            get { return this.value; }
        }
    }
}
