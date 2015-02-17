//---------------------------------------------------------------------
// <copyright file="CollectionDataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Collection data type
    /// </summary>
    public sealed class CollectionDataType : DataType, IEquatable<CollectionDataType>
    {
        /// <summary>
        /// Initializes a new instance of the CollectionDataType class.
        /// </summary>
        internal CollectionDataType()
            : this(true, null)
        {
        }

        private CollectionDataType(bool isNullable, DataType elementDataType)
            : base(isNullable)
        {
            this.ElementDataType = elementDataType;
        }

        /// <summary>
        /// Gets the Element data type
        /// </summary>
        public DataType ElementDataType { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="CollectionDataType" /> with the specified nullability.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the new type will be nullable.</param>
        /// <returns>New instance of <see cref="CollectionDataType"/> with specified nullability.</returns>
        public CollectionDataType Nullable(bool isNullable)
        {
            return new CollectionDataType(isNullable, this.ElementDataType);
        }

        /// <summary>
        /// Creates a new instance of <see cref="CollectionDataType"/> which has the same properties as
        /// this instance, except its element is of elementDataType.
        /// </summary>
        /// <param name="elementDataType">the element's data type</param>
        /// <returns>new instance of <see cref="CollectionDataType"/></returns>
        public CollectionDataType WithElementDataType(DataType elementDataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(elementDataType, "elementDataType");

            return new CollectionDataType(this.IsNullable, elementDataType);
        }

        /// <summary>
        /// Determines whether this data type is equal to another data type.
        /// </summary>
        /// <param name="other">The other data type.</param>
        /// <returns>True if this <see cref="DataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public override bool Equals(DataType other)
        {
            var otherCdt = other as CollectionDataType;
            return this.Equals(otherCdt);
        }

        /// <summary>
        /// Determines whether this collection data type is equal to another collection data type.
        /// </summary>
        /// <param name="other">The other collection data type.</param>
        /// <returns>True if this <see cref="CollectionDataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public bool Equals(CollectionDataType other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.IsNullable == other.IsNullable) && object.Equals(this.ElementDataType, other.ElementDataType);
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
            int hashCode = this.IsNullable ? 1234 : 5678;
            if (this.ElementDataType != null)
            {
                hashCode ^= this.ElementDataType.GetHashCode();
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
            if (this.ElementDataType != null)
            {
                sb.Append(this.ElementDataType.ToString());
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
            throw new TaupoNotSupportedException("Taupo does not support determining whether value is compatible with collection data type yet.");
        }
    }
}
