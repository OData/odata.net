//---------------------------------------------------------------------
// <copyright file="TimeOfDayDataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Time Of Day data type.
    /// </summary>
    public class TimeOfDayDataType : PrimitiveDataType
    {
        /// <summary>
        /// Initializes a new instance of the TimeOfDayDataType class.
        /// </summary>
        internal TimeOfDayDataType()
            : this(false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TimeOfDayDataType class.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c>, the type is nullable.</param>
        /// <param name="facets">The facets.</param>
        private TimeOfDayDataType(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets)
            : base(isNullable, facets)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="TimeOfDayDataType"/> which has the same facets as
        /// this instance, except for <see cref="TimePrecisionFacet"/> which is set to the specified value.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>
        /// New instance of <see cref="TimeOfDayDataType"/> with the new facet.
        /// </returns>
        public TimeOfDayDataType WithPrecision(int precision)
        {
            return this.WithFacet(new TimePrecisionFacet(precision));
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
            return new TimeOfDayDataType(isNullable, facets);
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
            if (value is TimeSpan)
            {
                var ts = (TimeSpan)value;
                if (ts.TotalHours < 0 || ts.TotalHours > 24)
                {
                    return false;
                }

                return true;
            }
            
            return false;
        }
    }
}
