//---------------------------------------------------------------------
// <copyright file="EnumMember.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Represents a Member of <see cref="EnumType"/>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection.")]
    [System.Diagnostics.DebuggerDisplay("Member Name={this.Name} Value={this.Value}")]
    public class EnumMember : AnnotatedItem
    {
        /// <summary>
        /// Initializes a new instance of the EnumMember class.
        /// </summary>
        public EnumMember()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EnumMember class with a given name.
        /// </summary>
        /// <param name="name">Enum Member name</param>
        public EnumMember(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EnumMember class with given name and value.
        /// </summary>
        /// <param name="name">Enum Membmer name</param>
        /// <param name="value">Enum Member value string representation.</param>
        public EnumMember(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets name of the Enum Member.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the string representation of the value of the Enum member.
        /// </summary>
        public object Value { get; set; }
    }
}
