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

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL String type.
    /// </summary>
    internal class CsdlStringTypeReference : CsdlPrimitiveTypeReference
    {
        private readonly bool? isFixedLength;
        private readonly bool isUnbounded;
        private readonly int? maxLength;
        private readonly bool? isUnicode;
        private readonly string collation;

        public CsdlStringTypeReference(bool? isFixedLength, bool isUnbounded, int? maxLength, bool? isUnicode, string collation, string typeName, bool isNullable, CsdlLocation location)
            : base(EdmPrimitiveTypeKind.String, typeName, isNullable, location)
        {
            this.isFixedLength = isFixedLength;
            this.isUnbounded = isUnbounded;
            this.maxLength = maxLength;
            this.isUnicode = isUnicode;
            this.collation = collation;
        }

        public bool? IsFixedLength
        {
            get { return this.isFixedLength; }
        }

        public bool IsUnbounded
        {
            get { return this.isUnbounded; }
        }

        public int? MaxLength
        {
            get { return this.maxLength; }
        }

        public bool? IsUnicode
        {
            get { return this.isUnicode; }
        }

        public string Collation
        {
            get { return this.collation; }
        }
    }
}
