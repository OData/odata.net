//---------------------------------------------------------------------
// <copyright file="RowDataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Row data type (or anonymous structural type)
    /// </summary>
    public class RowDataType : DataType, IEquatable<RowDataType>
    {
        /// <summary>
        /// Initializes a new instance of the RowDataType class.
        /// </summary>
        internal RowDataType()
            : this(true, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RowDataType class with the specified definition.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the type is nullable.</param>
        /// <param name="definition">The definition of row type</param>
        private RowDataType(bool isNullable, RowType definition)
            : base(isNullable)
        {
            this.Definition = definition;
        }

        /// <summary>
        /// Gets the row type definition
        /// </summary>
        public RowType Definition { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="RowDataType" /> with the specified nullability.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the new type will be nullable.</param>
        /// <returns>New instance of <see cref="RowDataType"/> with specified nullability.</returns>
        public RowDataType Nullable(bool isNullable)
        {
            return new RowDataType(isNullable, this.Definition);
        }

        /// <summary>
        /// Creates a new instance of <see cref="RowDataType"/> which has the same properties as
        /// this instance, except it will have the specified definition.
        /// </summary>
        /// <param name="rowTypeDefinition">The row type definition.</param>
        /// <returns>New instance of <see cref="RowDataType"/>.</returns>
        public RowDataType WithDefinition(RowType rowTypeDefinition)
        {
            ExceptionUtilities.CheckArgumentNotNull(rowTypeDefinition, "rowTypeDefinition");

            return new RowDataType(this.IsNullable, rowTypeDefinition);
        }

        /// <summary>
        /// Determines whether this data type is equal to another data type.
        /// </summary>
        /// <param name="other">The other data type.</param>
        /// <returns>True if this <see cref="DataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public override bool Equals(DataType other)
        {
            var otherRdt = other as RowDataType;
            return this.Equals(otherRdt);
        }

        /// <summary>
        /// Determines whether this row type is equal to another row type.
        /// </summary>
        /// <param name="other">The other row type.</param>
        /// <returns>True if this <see cref="RowDataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public bool Equals(RowDataType other)
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
            // TODO: remove this entire class
            throw new TaupoInvalidOperationException("This method is not supported.");
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
                sb.Append(string.Join(", ", this.Definition.Properties.Select(p => p.Name + " " + p.PropertyType.ToString()).ToArray()));
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
            throw new TaupoNotSupportedException("Taupo does not support determining whether value is compatible with row data type yet.");
        }
    }
}
