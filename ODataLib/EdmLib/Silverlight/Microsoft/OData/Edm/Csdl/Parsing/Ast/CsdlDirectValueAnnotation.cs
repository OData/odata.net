//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL annotation.
    /// </summary>
    internal class CsdlDirectValueAnnotation : CsdlElement
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly string value;
        private readonly bool isAttribute;

        public CsdlDirectValueAnnotation(string namespaceName, string name, string value, bool isAttribute, CsdlLocation location)
            : base(location)
        {
            this.namespaceName = namespaceName;
            this.name = name;
            this.value = value;
            this.isAttribute = isAttribute;
        }

        public string NamespaceName
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Value
        {
            get { return this.value; }
        }

        public bool IsAttribute
        {
            get { return this.isAttribute; }
        }
    }
}
