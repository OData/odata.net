//---------------------------------------------------------------------
// <copyright file="EnumDataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    /// <summary>
    /// Enum data type
    /// </summary>
    public class EnumDataType : PrimitiveDataType, IEquatable<EnumDataType>
    {
        private PrimitiveDataType cachedUnderlyingTypeInEdm;
        
        /// <summary>
        /// Initializes a new instance of the EnumDataType class.
        /// </summary>
        internal EnumDataType()
            : this(true, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EnumDataType class with the specified definition.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the type is nullable.</param>
        /// <param name="facets">The facets.</param>
        /// <param name="definition">The definition of enum type</param>
        private EnumDataType(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets, EnumType definition)
            : base(isNullable, facets)
        {
            this.Definition = definition;
        }

        /// <summary>
        /// Gets the enum type definition
        /// </summary>
        public EnumType Definition { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="EnumDataType" /> with the specified nullability.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the new type will be nullable.</param>
        /// <returns>New instance of <see cref="EnumDataType"/> with specified nullability.</returns>
        public EnumDataType Nullable(bool isNullable)
        {
            return new EnumDataType(isNullable, this.Facets, this.Definition);
        }

        /// <summary>
        /// Creates a new instance of <see cref="EnumDataType"/> which has the same properties as
        /// this instance, except it references given enum type.
        /// </summary>
        /// <param name="enumTypeName">Name of the enum type.</param>
        /// <returns>New instance of <see cref="EnumDataType"/>.</returns>
        public EnumDataType WithName(string enumTypeName)
        {
            return this.WithDefinition(new EnumTypeReference(enumTypeName));
        }

        /// <summary>
        /// Creates a new instance of <see cref="EnumDataType"/> which has the same properties as
        /// this instance, except it references given enum type.
        /// </summary>
        /// <param name="enumTypeNamespace">The enum type namespace.</param>
        /// <param name="enumTypeName">Name of the enum type.</param>
        /// <returns>
        /// New instance of <see cref="EnumDataType"/>.
        /// </returns>
        public EnumDataType WithName(string enumTypeNamespace, string enumTypeName)
        {
            return this.WithDefinition(new EnumTypeReference(enumTypeNamespace, enumTypeName));
        }

        /// <summary>
        /// Creates a new instance of <see cref="EnumDataType"/> which has the same properties as
        /// this instance, except it will have the specified definition.
        /// </summary>
        /// <param name="enumTypeDefinition">The enum type definition.</param>
        /// <returns>New instance of <see cref="EnumDataType"/>.</returns>
        public EnumDataType WithDefinition(EnumType enumTypeDefinition)
        {
            ExceptionUtilities.CheckArgumentNotNull(enumTypeDefinition, "enumTypeDefinition");

            return new EnumDataType(this.IsNullable, this.Facets, enumTypeDefinition);
        }

        /// <summary>
        /// Determines whether this data type is equal to another data type.
        /// </summary>
        /// <param name="other">The other data type.</param>
        /// <returns>True if this <see cref="DataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public override bool Equals(DataType other)
        {
            var otherEnum = other as EnumDataType;
            return this.Equals(otherEnum);
        }

        /// <summary>
        /// Determines whether this enum type is equal to another enum type.
        /// </summary>
        /// <param name="other">The other enum type.</param>
        /// <returns>True if this <see cref="EnumDataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public bool Equals(EnumDataType other)
        {
            if (other == null)
            {
                return false;
            }

            return object.Equals(this.Definition, other.Definition) && this.Equals((PrimitiveDataType)other);
        }

        /// <summary>
        /// Gets an Edm <see cref="PrimitiveDataType"/> that represents the underlying type of the enum.
        /// </summary>
        /// <returns>The <see cref="PrimitiveDataType"/> corresponding to the underlying type of the enum.</returns>
        public PrimitiveDataType GetUnderlyingTypeInEdm()
        {
            if (this.cachedUnderlyingTypeInEdm == null)
            {
                PrimitiveDataType underlyingTypeInEdm;

                if (this.Definition.UnderlyingType == null)
                {
                    underlyingTypeInEdm = EdmDataTypes.Int32;
                }
                else
                {
                    var allIntegerEdmTypes = EdmDataTypes.GetAllPrimitiveTypes(EdmVersion.Latest).OfType<IntegerDataType>();
                    underlyingTypeInEdm = allIntegerEdmTypes.SingleOrDefault(i => i.GetFacet<PrimitiveClrTypeFacet>().Value == this.Definition.UnderlyingType);
                    ExceptionUtilities.Assert(underlyingTypeInEdm != null, "Invalid underlying type for an Enum data type");
                }

                foreach (var facet in this.Facets)
                {
                    underlyingTypeInEdm = underlyingTypeInEdm.WithFacet(facet);
                }

                this.cachedUnderlyingTypeInEdm = underlyingTypeInEdm;
            }

            return this.cachedUnderlyingTypeInEdm;
        }

        /// <summary>
        /// Accepts the specified visitor by calling its Visit method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="visitor">The visitor.</param>
        /// <returns>Visitor-specific value.</returns>
        public override TValue Accept<TValue>(IPrimitiveDataTypeVisitor<TValue> visitor)
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
            int hashCode = base.GetHashCode();
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
            sb.Append(base.ToString());
            sb.Append("(");
            if (this.Definition != null)
            {
                sb.Append(this.Definition.FullName);
            }

            sb.Append(")");
            return sb.ToString();
        }
 
        /// <summary>
        /// Creates a new type based on this type with the specified nullability flag and facets.
        /// </summary>
        /// <param name="isNullable">Nullability flag for the new type.</param>
        /// <param name="facets">List of facets for the new type.</param>
        /// <returns>
        /// Newly created <see cref="PrimitiveDataType"/>.
        /// </returns>
        protected internal override PrimitiveDataType Create(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets)
        {
            return new EnumDataType(isNullable, facets, this.Definition);
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
            throw new TaupoNotSupportedException("Taupo does not support determining whether value is compatible with enum data type yet.");
        }
    }
}
