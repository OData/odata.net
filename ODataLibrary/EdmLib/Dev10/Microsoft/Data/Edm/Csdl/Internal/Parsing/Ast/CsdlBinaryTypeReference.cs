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

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL Binary type.
    /// </summary>
    internal class CsdlBinaryTypeReference : CsdlPrimitiveTypeReference
    {
        private readonly bool? isFixedLength;
        private readonly bool isMaxMaxLength;
        private readonly int?maxLength;

        public CsdlBinaryTypeReference(bool? isFixedLength, bool isMaxMaxLength, int? maxLength, string typeName, bool isNullable, CsdlLocation location)
            : base(EdmPrimitiveTypeKind.Binary, typeName, isNullable, location)
        {
            this.isFixedLength = isFixedLength;
            this.isMaxMaxLength = isMaxMaxLength;
            this.maxLength = maxLength;
        }

        public bool? IsFixedLength
        {
            get { return this.isFixedLength; }
        }

        public bool IsMaxMaxLength
        {
            get { return this.isMaxMaxLength; }
        }

        public int? MaxLength
        {
            get { return this.maxLength; }
        }
    }
}
