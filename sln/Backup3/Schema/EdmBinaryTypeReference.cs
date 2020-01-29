//---------------------------------------------------------------------
// <copyright file="EdmBinaryTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM binary type.
    /// </summary>
    public class EdmBinaryTypeReference : EdmPrimitiveTypeReference, IEdmBinaryTypeReference
    {
        private readonly bool isUnbounded;
        private readonly int? maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmBinaryTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmBinaryTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmBinaryTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="isUnbounded">Denotes whether the max length is the maximum allowed value.</param>
        /// <param name="maxLength">Maximum length of a value of this type.</param>
        public EdmBinaryTypeReference(IEdmPrimitiveType definition, bool isNullable, bool isUnbounded, int? maxLength)
            : base(definition, isNullable)
        {
            if (isUnbounded && maxLength != null)
            {
                throw new InvalidOperationException(Edm.Strings.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
            }

            this.isUnbounded = isUnbounded;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Gets a value indicating whether this type specifies the maximum allowed length.
        /// </summary>
        public bool IsUnbounded
        {
            get { return this.isUnbounded; }
        }

        /// <summary>
        /// Gets the maximum length of this type.
        /// </summary>
        public int? MaxLength
        {
            get { return this.maxLength; }
        }
    }
}
