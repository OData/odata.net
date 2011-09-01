//   Copyright 2011 Microsoft Corporation
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
    /// Represents a CSDL navigation property.
    /// </summary>
    internal class CsdlNavigationProperty : CsdlNamedElement
    {
        private readonly string relationship;
        private readonly string toRole;
        private readonly string fromRole;

        public CsdlNavigationProperty(string name, string relationship, string toRole, string fromRole, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.relationship = relationship;
            this.toRole = toRole;
            this.fromRole = fromRole;
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
    }
}
