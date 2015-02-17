//---------------------------------------------------------------------
// <copyright file="ReferenceDataType.cs" company="Microsoft">
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
    /// Reference data type (which holds reference to an entity type)
    /// </summary>
    public class ReferenceDataType : DataType, IEquatable<ReferenceDataType>
    {
        /// <summary>
        /// Initializes a new instance of the ReferenceDataType class.
        /// </summary>
        internal ReferenceDataType()
            : this(true, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ReferenceDataType class with the specified entity type to reference
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the type is nullable.</param>
        /// <param name="entityType">The entity type of the reference.</param>
        private ReferenceDataType(bool isNullable, EntityType entityType)
            : base(isNullable)
        {
            this.EntityType = entityType;
        }

        /// <summary>
        /// Gets the entity type of the reference
        /// </summary>
        public EntityType EntityType { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceDataType" /> with the specified nullability.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the new type will be nullable.</param>
        /// <returns>New instance of <see cref="ReferenceDataType"/> with specified nullability.</returns>
        public ReferenceDataType Nullable(bool isNullable)
        {
            return new ReferenceDataType(isNullable, this.EntityType);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceDataType"/> which has the same properties as
        /// this instance, except it will reference the given entity type.
        /// </summary>
        /// <param name="entityType">The entity type of the reference</param>
        /// <returns>New instance of <see cref="ReferenceDataType"/>.</returns>
        public ReferenceDataType WithEntityType(EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            return new ReferenceDataType(this.IsNullable, entityType);
        }

        /// <summary>
        /// Determines whether this data type is equal to another data type.
        /// </summary>
        /// <param name="other">The other data type.</param>
        /// <returns>True if this <see cref="DataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public override bool Equals(DataType other)
        {
            var otherRdt = other as ReferenceDataType;
            return this.Equals(otherRdt);
        }

        /// <summary>
        /// Determines whether this reference type is equal to another reference type.
        /// </summary>
        /// <param name="other">The other reference type.</param>
        /// <returns>True if this <see cref="ReferenceDataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public bool Equals(ReferenceDataType other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.IsNullable == other.IsNullable) && object.Equals(this.EntityType, other.EntityType);
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
            if (this.EntityType != null)
            {
                hashCode ^= this.EntityType.GetHashCode();
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
            if (this.EntityType != null)
            {
                sb.Append(this.EntityType.FullName);
            }

            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the specified value is compatible with the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Currently throw TaupoNotSupportedException
        /// </returns>
        protected override bool IsValueCompatible(object value)
        {
            throw new TaupoNotSupportedException("Taupo does not support determining whether value is compatible with reference data type yet.");
        }
    }
}
