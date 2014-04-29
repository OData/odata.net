//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL entity type.
    /// </summary>
    internal class CsdlEntityType : CsdlNamedStructuredType
    {
        private readonly CsdlKey key;
        private readonly bool hasStream;
        private readonly List<CsdlNavigationProperty> navigationProperties;

        public CsdlEntityType(string name, string baseTypeName, bool isAbstract, bool isOpen, bool hasStream, CsdlKey key, IEnumerable<CsdlProperty> properties, IEnumerable<CsdlNavigationProperty> navigationProperties, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, baseTypeName, isAbstract, isOpen, properties, documentation, location)
        {
            this.key = key;
            this.hasStream = hasStream;

            this.navigationProperties = new List<CsdlNavigationProperty>(navigationProperties);
        }

        public IEnumerable<CsdlNavigationProperty> NavigationProperties
        {
            get { return this.navigationProperties; }
        }

        public CsdlKey Key
        {
            get { return this.key; }
        }

        public bool HasStream 
        {
            get { return this.hasStream; }
        }
    }
}
