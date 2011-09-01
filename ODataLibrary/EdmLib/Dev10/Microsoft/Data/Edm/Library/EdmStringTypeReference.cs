//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM string type.
    /// </summary>
    public class EdmStringTypeReference : EdmPrimitiveTypeReference, IEdmStringTypeReference
    {
        private readonly bool isMaxMaxLength;
        private readonly int? maxLength;
        private readonly bool? isFixedLength;
        private readonly bool? isUnicode;
        private readonly string collation;

        /// <summary>
        /// Initializes a new instance of the EdmStringTypeReference class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmStringTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, false, null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EdmStringTypeReference class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="isMaxMaxLength">Denotes whether the max length is the maximum allowed value.</param>
        /// <param name="maxLength">Maximum length of a value of this type.</param>
        /// <param name="isFixedLength">Denotes whether the length can vary. </param>
        /// <param name="isUnicode">Denotes if string is encoded using Unicode.</param>
        /// <param name="collation">Indicates the collation string to be used by the underlying store.</param>
        public EdmStringTypeReference(IEdmPrimitiveType definition, bool isNullable, bool isMaxMaxLength, int? maxLength, bool? isFixedLength, bool? isUnicode, string collation)
            : base(definition, isNullable)
        {
            // TODO: add check that if maxLength is not null, then maxMaxLength is false,
            // if maxMaxLength is true then maxLength is null.
            // In addition, add a validation rule
            this.isMaxMaxLength = isMaxMaxLength;
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
        /// Gets a value indicating whether this string type specifies the maximum allowed max length.
        /// </summary>
        public bool IsMaxMaxLength
        {
            get { return this.isMaxMaxLength; }
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
