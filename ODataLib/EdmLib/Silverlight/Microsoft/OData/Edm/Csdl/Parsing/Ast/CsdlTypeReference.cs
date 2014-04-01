//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Base type for the two kinds of type reference: <see cref="CsdlNamedTypeReference"/> and <see cref="CsdlExpressionTypeReference"/>.
    /// </summary>
    internal abstract class CsdlTypeReference : CsdlElement
    {
        private readonly bool isNullable;

        protected CsdlTypeReference(bool isNullable, CsdlLocation location)
            : base(location)
        {
            this.isNullable = isNullable;
        }

        public bool IsNullable
        {
            get { return this.isNullable; }
        }
    }
}
