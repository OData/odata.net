//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
