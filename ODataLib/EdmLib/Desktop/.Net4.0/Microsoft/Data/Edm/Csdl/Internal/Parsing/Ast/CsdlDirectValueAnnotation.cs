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
