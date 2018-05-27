//---------------------------------------------------------------------
// <copyright file="EdmTemporalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM temporal (Duration, DateTime, DateTimeOffset) type.
    /// </summary>
    public class EdmTemporalTypeReference : EdmPrimitiveTypeReference, IEdmTemporalTypeReference
    {
        private readonly int? precision;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTemporalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmTemporalTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTemporalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="precision">Precision of values with this type.</param>
        public EdmTemporalTypeReference(IEdmPrimitiveType definition, bool isNullable, int? precision)
            : base(definition, isNullable)
        {
            this.precision = precision;
        }

        /// <summary>
        /// Gets the precision of this temporal type.
        /// </summary>
        public int? Precision
        {
            get { return this.precision; }
        }
    }
}
