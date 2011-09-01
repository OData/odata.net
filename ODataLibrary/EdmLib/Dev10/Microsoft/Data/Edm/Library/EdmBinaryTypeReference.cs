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
    /// Represents a reference to an EDM binary type.
    /// </summary>
    public class EdmBinaryTypeReference : EdmPrimitiveTypeReference, IEdmBinaryTypeReference
    {
        private readonly bool isMaxMaxLength;
        private readonly int? maxLength;
        private readonly bool? isFixedLength;

        /// <summary>
        /// Initializes a new instance of the EdmBinaryTypeReference class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmBinaryTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, false, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EdmBinaryTypeReference class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="isMaxMaxLength">Denotes whether the max length is the maximum allowed value.</param>
        /// <param name="maxLength">Maximum length of a value of this type.</param>
        /// <param name="isFixedLength">Denotes whether the length can vary. </param>
        public EdmBinaryTypeReference(IEdmPrimitiveType definition, bool isNullable, bool isMaxMaxLength, int? maxLength, bool? isFixedLength)
            : base(definition, isNullable)
        {
            // TODO: add check that if maxLength is not null, then maxMaxLength is false,
            // if maxMaxLength is true then maxLength is null.
            // In addition, add a validation rule
            this.isMaxMaxLength = isMaxMaxLength;
            this.maxLength = maxLength;
            this.isFixedLength = isFixedLength;
        }

        /// <summary>
        /// Gets a value indicating whether this type specifies fixed length.
        /// </summary>
        public bool? IsFixedLength
        {
            get { return this.isFixedLength; }
        }

        /// <summary>
        /// Gets a value indicating whether this type specifies the maximum allowed max length.
        /// </summary>
        public bool IsMaxMaxLength
        {
            get { return this.isMaxMaxLength; }
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
