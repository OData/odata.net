//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL type definition.
    /// </summary>
    internal class CsdlTypeDefinition : CsdlNamedElement
    {
        private readonly string underlyingTypeName;
        private readonly int? maxLength;
        private readonly bool? isUnicode;
        private readonly int? precision;
        private readonly int? scale;
        private readonly int? srid;

        public CsdlTypeDefinition(string name, string underlyingTypeName, int? maxLength, bool? isUnicode, int? precision, int? scale, int? srid, CsdlLocation location)
            : base(name, /*documentation*/null, location)
        {
            this.underlyingTypeName = underlyingTypeName;
            this.maxLength = maxLength;
            this.isUnicode = isUnicode;
            this.precision = precision;
            this.scale = scale;
            this.srid = srid;
        }

        public string UnderlyingTypeName
        {
            get { return this.underlyingTypeName; }
        }

        public int? MaxLength
        {
            get { return this.maxLength; }
        }

        public bool? IsUnicode
        {
            get { return this.isUnicode; }
        }

        public int? Precision
        {
            get { return this.precision; }
        }

        public int? Scale
        {
            get { return this.scale; }
        }

        public int? Srid
        {
            get { return this.srid; }
        }
    }
}
