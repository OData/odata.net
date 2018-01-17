//---------------------------------------------------------------------
// <copyright file="EdmDecimalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using Microsoft.OData.Edm.Csdl;

    /// <summary>
    /// Represents a reference to an EDM decimal type.
    /// </summary>
    public class EdmDecimalTypeReference : EdmPrimitiveTypeReference, IEdmDecimalTypeReference
    {
        private readonly int? precision;
        private readonly int? scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDecimalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmDecimalTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, null, CsdlConstants.Default_Scale)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDecimalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="precision">Precision of values with this type.</param>
        /// <param name="scale">Scale of values with this type.</param>
        public EdmDecimalTypeReference(IEdmPrimitiveType definition, bool isNullable, int? precision, int? scale)
            : base(definition, isNullable)
        {
            this.precision = precision;
            this.scale = scale;
        }

        /// <summary>
        /// Gets the precision of this type.
        /// </summary>
        public int? Precision
        {
            get { return this.precision; }
        }

        /// <summary>
        /// Gets the scale of this type.
        /// </summary>
        public int? Scale
        {
            get { return this.scale; }
        }
    }
}
