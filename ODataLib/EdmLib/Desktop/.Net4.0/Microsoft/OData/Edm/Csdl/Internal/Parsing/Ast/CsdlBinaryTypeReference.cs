//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL Binary type.
    /// </summary>
    internal class CsdlBinaryTypeReference : CsdlPrimitiveTypeReference
    {
        private readonly bool? isFixedLength;
        private readonly bool isUnbounded;
        private readonly int?maxLength;

        public CsdlBinaryTypeReference(bool? isFixedLength, bool isUnbounded, int? maxLength, string typeName, bool isNullable, CsdlLocation location)
            : base(EdmPrimitiveTypeKind.Binary, typeName, isNullable, location)
        {
            this.isFixedLength = isFixedLength;
            this.isUnbounded = isUnbounded;
            this.maxLength = maxLength;
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
    }
}
