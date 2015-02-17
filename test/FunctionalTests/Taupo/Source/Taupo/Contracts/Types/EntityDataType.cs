//---------------------------------------------------------------------
// <copyright file="EntityDataType.cs" company="Microsoft">
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
    /// Entity data type
    /// </summary>
    public sealed class EntityDataType : DataType, IEquatable<EntityDataType>
    {
        /// <summary>
        /// Initializes a new instance of the EntityDataType class.
        /// </summary>
        internal EntityDataType()
            : this(true, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntityDataType class.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the type is nullable.</param>
        /// <param name="definition">The type definition.</param>
        private EntityDataType(bool isNullable, EntityType definition)
            : base(isNullable)
        {
            this.Definition = definition;
        }

        /// <summary>
        /// Gets the <see cref="EntityType"/> definition.
        /// </summary>
        /// <value>The entity type definition.</value>
        public EntityType Definition { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="EntityDataType" /> with the specified nullability.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the new type will be nullable.</param>
        /// <returns>New instance of <see cref="EntityDataType"/> with specified nullability.</returns>
        public EntityDataType Nullable(bool isNullable)
        {
            return new EntityDataType(isNullable, this.Definition);
        }

        /// <summary>
        /// Creates a new instance of <see cref="EntityDataType"/> which has the same properties as
        /// this instance, except it references given entity type.
        /// </summary>
        /// <param name="entityTypeDefinition">The entity type definition.</param>
        /// <returns>
        /// New instance of <see cref="EntityDataType"/>.
        /// </returns>
        public EntityDataType WithDefinition(EntityType entityTypeDefinition)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityTypeDefinition, "entityTypeDefinition");

            return new EntityDataType(this.IsNullable, entityTypeDefinition);
        }

        /// <summary>
        /// Creates a new instance of <see cref="EntityDataType"/> which has the same properties as
        /// this instance, except its definition has the given name.
        /// </summary>
        /// <param name="entityTypeName">Name of the complex type.</param>
        /// <returns>New instance of <see cref="EntityDataType"/>.</returns>
        public EntityDataType WithName(string entityTypeName)
        {
            return this.WithDefinition(new EntityTypeReference(entityTypeName));
        }

        /// <summary>
        /// Creates a new instance of <see cref="EntityDataType"/> which has the same properties as
        /// this instance, except it its definition has the given namespace and name
        /// </summary>
        /// <param name="entityTypeNamespace">The entity type namespace.</param>
        /// <param name="entityTypeName">The entity type name.</param>
        /// <returns>New instance of <see cref="EntityDataType"/>.</returns>
        public EntityDataType WithName(string entityTypeNamespace, string entityTypeName)
        {
            return this.WithDefinition(new EntityTypeReference(entityTypeNamespace, entityTypeName));
        }

        /// <summary>
        /// Determines whether this data type is equal to another data type.
        /// </summary>
        /// <param name="other">The other data type.</param>
        /// <returns>True if this <see cref="DataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public override bool Equals(DataType other)
        {
            var otherEdt = other as EntityDataType;
            return this.Equals(otherEdt);
        }

        /// <summary>
        /// Determines whether this entity type is equal to another entity type.
        /// </summary>
        /// <param name="other">The other entity type.</param>
        /// <returns>True if this <see cref="EntityDataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public bool Equals(EntityDataType other)
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
        /// Currently throw TaupoNotSupportedException
        /// </returns>
        protected override bool IsValueCompatible(object value)
        {
            throw new TaupoNotSupportedException("Taupo does not support determining whether value is compatible with entity data type yet.");
        }
    }
}
