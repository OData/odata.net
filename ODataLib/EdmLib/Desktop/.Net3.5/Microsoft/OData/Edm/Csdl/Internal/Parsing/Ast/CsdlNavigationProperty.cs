//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL navigation property.
    /// </summary>
    internal class CsdlNavigationProperty : CsdlNamedElement
    {
        private readonly string relationship;
        private readonly string toRole;
        private readonly string fromRole;
        private readonly bool containsTarget;

        public CsdlNavigationProperty(string name, string relationship, string toRole, string fromRole, bool containsTarget, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.relationship = relationship;
            this.toRole = toRole;
            this.fromRole = fromRole;
            this.containsTarget = containsTarget;
        }

        public string Relationship
        {
            get { return this.relationship; }
        }

        public string ToRole
        {
            get { return this.toRole; }
        }

        public string FromRole
        {
            get { return this.fromRole; }
        }

        public bool ContainsTarget
        {
            get { return this.containsTarget; }
        }
    }
}
