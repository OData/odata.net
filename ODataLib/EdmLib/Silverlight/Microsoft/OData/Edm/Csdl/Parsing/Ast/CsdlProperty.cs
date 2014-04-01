//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL property.
    /// </summary>
    internal class CsdlProperty : CsdlNamedElement
    {
        private readonly CsdlTypeReference type;
        private readonly string defaultValue;
        private readonly bool isFixedConcurrency;

        public CsdlProperty(string name, CsdlTypeReference type, bool isFixedConcurrency, string defaultValue, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.type = type;
            this.isFixedConcurrency = isFixedConcurrency;
            this.defaultValue = defaultValue;
        }

        public CsdlTypeReference Type
        {
            get { return this.type; }
        }

        public string DefaultValue
        {
            get { return this.defaultValue; }
        }

        public bool IsFixedConcurrency
        {
            get { return this.isFixedConcurrency; }
        }
    }
}
