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
    /// Represents a CSDL association set end.
    /// </summary>
    internal class CsdlAssociationSetEnd : CsdlElementWithDocumentation
    {
        private readonly string role;
        private readonly string entitySet;

        public CsdlAssociationSetEnd(string role, string entitySet, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.role = role;
            this.entitySet = entitySet;
        }

        public string Role
        {
            get { return this.role; }
        }

        public string EntitySet
        {
            get { return this.entitySet; }
        }
    }
}
