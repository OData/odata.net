//---------------------------------------------------------------------
// <copyright file="StringDataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System.Collections.Generic;

    /// <summary>
    /// String Data Type.
    /// </summary>
    public sealed class StringDataType : PrimitiveDataType
    {
        /// <summary>
        /// Initializes a new instance of the StringDataType class.
        /// </summary>
        internal StringDataType()
            : this(false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StringDataType class.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c>, the type is nullable.</param>
        /// <param name="facets">The facets.</param>
        private StringDataType(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets)
            : base(isNullable, facets)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="StringDataType"/> which has the same facets as
        /// this instance, except for maximum length which is set to the specified value.
        /// </summary>
        /// <param name="maxLength">Maxumim length.</param>
        /// <returns>
        /// New instance of <see cref="StringDataType"/> with specified maximum length.
        /// </returns>
        public StringDataType WithMaxLength(int maxLength)
        {
            return this.WithFacet(new MaxLengthFacet(maxLength));
        }

        /// <summary>
        /// Creates a new instance of <see cref="StringDataType"/> which has the same facets as
        /// this instance, except for <see cref="IsUnicodeFacet"/> which is set to the specified value.
        /// </summary>
        /// <param name="isUnicode">If set to <c>true</c> the type will support Unicode.</param>
        /// <returns>
        /// New instance of <see cref="StringDataType"/> with the new facet.
        /// </returns>
        public StringDataType WithUnicodeSupport(bool isUnicode)
        {
            return this.WithFacet(new IsUnicodeFacet(isUnicode));
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
        /// Creates a new type based on this type with the specified nullability flag and facets.
        /// </summary>
        /// <param name="isNullable">Nullability flag for the new type.</param>
        /// <param name="facets">List of facets for the new type.</param>
        /// <returns>
        /// Newly created <see cref="PrimitiveDataType"/>.
        /// </returns>
        protected internal override PrimitiveDataType Create(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets)
        {
            return new StringDataType(isNullable, facets);
        }

        /// <summary>
        /// Determines whether the specified value is compatible with the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A value of <c>true</c> if the value is compatible with the type; otherwise <c>false</c>.
        /// </returns>
        protected override bool IsValueCompatible(object value)
        {
            var strVal = value as string;
            if (strVal == null)
            {
                return false;
            }

            MaxLengthFacet mlf;

            if (this.TryGetFacet(out mlf))
            {
                if (strVal.Length > mlf.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
