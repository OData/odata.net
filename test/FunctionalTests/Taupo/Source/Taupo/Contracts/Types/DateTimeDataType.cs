//---------------------------------------------------------------------
// <copyright file="DateTimeDataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Date/Time data type - represents point-in-time values but not time of day or time span values.
    /// </summary>
    public class DateTimeDataType : PrimitiveDataType
    {
        /// <summary>
        /// Initializes a new instance of the DateTimeDataType class.
        /// </summary>
        internal DateTimeDataType()
            : this(false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DateTimeDataType class.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c>, the type is nullable.</param>
        /// <param name="facets">The facets.</param>
        private DateTimeDataType(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets)
            : base(isNullable, facets)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DateTimeDataType"/> which has the same facets as
        /// this instance, except for <see cref="TimePrecisionFacet"/> which is set to the specified value.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>
        /// New instance of <see cref="DateTimeDataType"/> with the new facet.
        /// </returns>
        public DateTimeDataType WithPrecision(int precision)
        {
            return this.WithFacet(new TimePrecisionFacet(precision));
        }

        /// <summary>
        /// Creates a new instance of <see cref="DateTimeDataType"/> which has the same facets as
        /// this instance, except for <see cref="HasTimeZoneOffsetFacet"/> which is set to the specified value.
        /// </summary>
        /// <param name="hasTimeZoneOffset">If set to <c>true</c> the type will have time zone offset.</param>
        /// <returns>
        /// New instance of <see cref="DateTimeDataType"/> with the new facet.
        /// </returns>
        public DateTimeDataType WithTimeZoneOffset(bool hasTimeZoneOffset)
        {
            return this.WithFacet(new HasTimeZoneOffsetFacet(hasTimeZoneOffset));
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
            return new DateTimeDataType(isNullable, facets);
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
            return value is DateTime || value is DateTimeOffset;
        }
    }
}
