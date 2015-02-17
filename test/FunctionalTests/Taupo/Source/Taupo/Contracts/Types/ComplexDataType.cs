//---------------------------------------------------------------------
// <copyright file="ComplexDataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Complex data type.
    /// </summary>
    public sealed class ComplexDataType : DataType, IEquatable<ComplexDataType>
    {
        /// <summary>
        /// Initializes a new instance of the ComplexDataType class.
        /// </summary>
        internal ComplexDataType()
            : this(false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComplexDataType class.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the type is nullable.</param>
        /// <param name="definition">The type definition.</param>
        private ComplexDataType(bool isNullable, ComplexType definition)
            : base(isNullable)
        {
            this.Definition = definition;
        }

        /// <summary>
        /// Gets the <see cref="ComplexType"/> definition.
        /// </summary>
        /// <value>The complex type definition.</value>
        public ComplexType Definition { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ComplexDataType" /> with the specified nullability.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the new type will be nullable.</param>
        /// <returns>New instance of <see cref="ComplexDataType"/> with specified nullability.</returns>
        public ComplexDataType Nullable(bool isNullable)
        {
            return new ComplexDataType(isNullable, this.Definition);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ComplexDataType"/> which has the same properties as
        /// this instance, except it is not nullable.
        /// </summary>
        /// <returns>New instance of <see cref="ComplexDataType"/> which is non-nullable.</returns>
        public ComplexDataType NotNullable()
        {
            return this.Nullable(false);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ComplexDataType"/> which has the same properties as
        /// this instance, except it is nullable.
        /// </summary>
        /// <returns>New instance of <see cref="ComplexDataType"/> which is nullable.</returns>
        public ComplexDataType Nullable()
        {
            return this.Nullable(true);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ComplexDataType"/> which has the same properties as
        /// this instance, except it references given complex type.
        /// </summary>
        /// <param name="complexTypeName">Name of the complex type.</param>
        /// <returns>New instance of <see cref="ComplexDataType"/>.</returns>
        public ComplexDataType WithName(string complexTypeName)
        {
            return this.WithDefinition(new ComplexTypeReference(complexTypeName));
        }

        /// <summary>
        /// Creates a new instance of <see cref="ComplexDataType"/> which has the same properties as
        /// this instance, except it references given complex type.
        /// </summary>
        /// <param name="complexTypeNamespace">The complex type namespace.</param>
        /// <param name="complexTypeName">Name of the complex type.</param>
        /// <returns>
        /// New instance of <see cref="ComplexDataType"/>.
        /// </returns>
        public ComplexDataType WithName(string complexTypeNamespace, string complexTypeName)
        {
            return this.WithDefinition(new ComplexTypeReference(complexTypeNamespace, complexTypeName));
        }

        /// <summary>
        /// Creates a new instance of <see cref="ComplexDataType"/> which has the same properties as
        /// this instance, except it references given complex type.
        /// </summary>
        /// <param name="complexTypeDefinition">The complex type definition.</param>
        /// <returns>
        /// New instance of <see cref="ComplexDataType"/>.
        /// </returns>
        public ComplexDataType WithDefinition(ComplexType complexTypeDefinition)
        {
            ExceptionUtilities.CheckArgumentNotNull(complexTypeDefinition, "complexTypeDefinition");

            if (ReferenceEquals(complexTypeDefinition, this.Definition))
            {
                return this;
            }

            return new ComplexDataType(this.IsNullable, complexTypeDefinition);
        }

        /// <summary>
        /// Determines whether this data type is equal to another data type.
        /// </summary>
        /// <param name="other">The other data type.</param>
        /// <returns>True if this <see cref="DataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public override bool Equals(DataType other)
        {
            var otherCdt = other as ComplexDataType;
            return this.Equals(otherCdt);
        }

        /// <summary>
        /// Determines whether this complex type is equal to another complex type.
        /// </summary>
        /// <param name="other">The other complex type.</param>
        /// <returns>True if this <see cref="ComplexDataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public bool Equals(ComplexDataType other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.IsNullable == other.IsNullable) && object.Equals(this.Definition, other.Definition);
        }

        /// <summary>
        /// Accepts the specified visitor by calling its Visit method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="visitor">The visitor.</param>
        /// <returns>Visitor-specific value.</returns>
        public override TValue Accept<TValue>(IDataTypeVisitor<TValue> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = this.IsNullable ? 12345 : 6789;
            if (this.Definition != null)
            {
                hashCode ^= this.Definition.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.GetType().Name);
            sb.Append("[");
            sb.Append("Nullable=");
            sb.Append(this.IsNullable);
            sb.Append("](");
            if (this.Definition != null)
            {
                sb.Append(this.Definition.FullName);
            }

            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the specified value is compatible with the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Currently returns true
        /// </returns>
        protected override bool IsValueCompatible(object value)
        {
            return true;
        }
    }
}
