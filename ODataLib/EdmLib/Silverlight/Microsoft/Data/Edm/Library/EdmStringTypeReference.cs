//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM string type.
    /// </summary>
    public class EdmStringTypeReference : EdmPrimitiveTypeReference, IEdmStringTypeReference
    {
        private readonly bool isUnbounded;
        private readonly int? maxLength;
        private readonly bool? isFixedLength;
        private readonly bool? isUnicode;
        private readonly string collation;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStringTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmStringTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, false, null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStringTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="isUnbounded">Denotes whether the max length is the maximum allowed value.</param>
        /// <param name="maxLength">Maximum length of a value of this type.</param>
        /// <param name="isFixedLength">Denotes whether the length can vary. </param>
        /// <param name="isUnicode">Denotes if string is encoded using Unicode.</param>
        /// <param name="collation">Indicates the collation string to be used by the underlying store.</param>
        public EdmStringTypeReference(IEdmPrimitiveType definition, bool isNullable, bool isUnbounded, int? maxLength, bool? isFixedLength, bool? isUnicode, string collation)
            : base(definition, isNullable)
        {
            if (isUnbounded && maxLength != null)
            {
                throw new InvalidOperationException(Edm.Strings.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
            }

            this.isUnbounded = isUnbounded;
            this.maxLength = maxLength;
            this.isFixedLength = isFixedLength;
            this.isUnicode = isUnicode;
            this.collation = collation;
        }

        /// <summary>
        /// Gets a value indicating whether this string type specifies fixed length.
        /// </summary>
        public bool? IsFixedLength
        {
            get { return this.isFixedLength; }
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

        /// <summary>
        /// Gets a string representing the collation of this string type.
        /// </summary>
        public string Collation
        {
            get { return this.collation; }
        }
    }
}
