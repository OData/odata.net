//---------------------------------------------------------------------
// <copyright file="EnumType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Enum type in EdmSchemaModel
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    [System.Diagnostics.DebuggerDisplay("EnumType {this.NamespaceName}.{this.Name} Members={this.Members.Count}")]
    public class EnumType : NamedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the EnumType class.
        /// </summary>
        public EnumType()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EnumType class with given name.
        /// </summary>
        /// <param name="name">Name of the Enum Type</param>
        public EnumType(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EnumType class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">Namespace of the Enum Type</param>
        /// <param name="name">Name of the Enum Type</param>
        public EnumType(string namespaceName, string name)
            : base(namespaceName, name)
        {
            this.Members = new List<EnumMember>();
        }
        
        /// <summary>
        /// Gets the members of the Enum type
        /// </summary>
        public IList<EnumMember> Members { get; private set; }

        /// <summary>
        /// Gets the model this type belongs to. If it is null, then the type has not been added to a model.
        /// </summary>
        public EntityModelSchema Model { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Enum is flags (supports bitwise operations)
        /// </summary>
        /// <remarks>
        /// value = true: The generated CSDL will have an IsFlags attribute set to true.
        /// value = false: The generated CSDL will have an IsFlags attribute set to false.
        /// value = null: The generated CSDL will not contain the IsFlags attribute.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "The name is the same as in product (attrubute in csdl)")]
        public bool? IsFlags { get; set; }

        /// <summary>
        /// Gets or sets the underlying type of the Enum.
        /// </summary>
        public Type UnderlyingType { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.EnumType"/>.
        /// </summary>
        /// <param name="enumTypeName">Name of the enum type.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator EnumType(string enumTypeName)
        {
            return new EnumTypeReference(enumTypeName);
        }

        /// <summary>
        /// Adds a new member to this Enum type
        /// </summary>
        /// <param name="member">The enum member to be added</param>
        public void Add(EnumMember member)
        {
            this.Members.Add(member);
        }

        /// <summary>
        /// Determines whether the specified <see cref="INamedItem"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="INamedItem"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="INamedItem"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(INamedItem other)
        {
            EnumType otherEnum = other as EnumType;
            if (otherEnum == null)
            {
                return false;
            }

            return (this.Name == otherEnum.Name) && (this.NamespaceName == otherEnum.NamespaceName);
        }
    }
}
