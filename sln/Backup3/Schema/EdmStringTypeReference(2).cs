//---------------------------------------------------------------------
// <copyright file="EdmStringTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM string type.
    /// </summary>
    public class EdmStringTypeReference : EdmPrimitiveTypeReference, IEdmStringTypeReference
    {
        private readonly bool isUnbounded;
        private readonly int? maxLength;
        private readonly bool? isUnicode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStringTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmStringTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, false, null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStringTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="isUnbounded">Denotes whether the max length is the maximum allowed value.</param>
        /// <param name="maxLength">Maximum length of a value of this type.</param>
        /// <param name="isUnicode">Denotes if string is encoded using Unicode.</param>
        public EdmStringTypeReference(IEdmPrimitiveType definition, bool isNullable, bool isUnbounded, int? maxLength, bool? isUnicode)
            : base(definition, isNullable)
        {
            if (isUnbounded && maxLength != null)
            {
                throw new InvalidOperationException(Edm.Strings.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
            }

            this.isUnbounded = isUnbounded;
            this.maxLength = maxLength;
            this.isUnicode = isUnicode;
        }

        /// <summary>
        /// Gets a value indicating whether this string type specifies the maximum allowed length.
        /// </summary>
        public bool IsUnbounded
        {
            get { return this.isUnbounded; }
        }

        /// <summary>
        /// Gets the maximum length of this string type.
        /// </summary>
        public int? MaxLength
        {
            get { return this.maxLength; }
        }

        /// <summary>
        /// Gets a value indicating whether this string type supports unicode encoding.
        /// </summary>
        public bool? IsUnicode
        {
            get { return this.isUnicode; }
        }
    }
}
