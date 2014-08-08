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
    /// Represents a CSDL association.
    /// </summary>
    internal class CsdlAssociation : CsdlNamedElement
    {
        private readonly CsdlReferentialConstraint constraint;
        private readonly CsdlAssociationEnd end1;
        private readonly CsdlAssociationEnd end2;

        public CsdlAssociation(string name, CsdlAssociationEnd end1, CsdlAssociationEnd end2, CsdlReferentialConstraint constraint, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.end1 = end1;
            this.end2 = end2;
            this.constraint = constraint;
        }

        public CsdlReferentialConstraint Constraint
        {
            get { return this.constraint; }
        }

        public CsdlAssociationEnd End1
        {
            get { return this.end1; }
        }

        public CsdlAssociationEnd End2
        {
            get { return this.end2; }
        }
    }
}
