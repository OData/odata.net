//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL referential constraint.
    /// </summary>
    internal class CsdlReferentialConstraint : CsdlElementWithDocumentation
    {
        private readonly string propertyName;
        private readonly string referencedPropertyName;

        public CsdlReferentialConstraint(string propertyName, string referencedPropertyName, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.propertyName = propertyName;
            this.referencedPropertyName = referencedPropertyName;
        }

        public string PropertyName
        {
            get { return this.propertyName; }
        }

        public string ReferencedPropertyName
        {
            get { return this.referencedPropertyName; }
        }
    }
}
