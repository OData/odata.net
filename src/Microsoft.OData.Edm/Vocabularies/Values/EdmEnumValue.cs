//---------------------------------------------------------------------
// <copyright file="EdmEnumValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM enumeration type value.
    /// </summary>
    public class EdmEnumValue : EdmValue, IEdmEnumValue
    {
        private readonly IEdmEnumMemberValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumValue"/> class.
        /// </summary>
        /// <param name="type">A reference to the enumeration type that describes this value.</param>
        /// <param name="member">The enumeration type value.</param>
        public EdmEnumValue(IEdmEnumTypeReference type, IEdmEnumMember member)
            : this(type, member.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumValue"/> class.
        /// </summary>
        /// <param name="type">A reference to the enumeration type that describes this value.</param>
        /// <param name="value">The underlying type value.</param>
        public EdmEnumValue(IEdmEnumTypeReference type, IEdmEnumMemberValue value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the underlying type value of the enumeration type.
        /// </summary>
        public IEdmEnumMemberValue Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Enum; }
        }
    }
}