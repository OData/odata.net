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
    /// Represents a CSDL association set.
    /// </summary>
    internal class CsdlAssociationSet : CsdlNamedElement
    {
        private readonly string association;
        private readonly CsdlAssociationSetEnd end1;
        private readonly CsdlAssociationSetEnd end2;

        public CsdlAssociationSet(string name, string association, CsdlAssociationSetEnd end1, CsdlAssociationSetEnd end2, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.association = association;
            this.end1 = end1;
            this.end2 = end2;
        }

        public string Association
        {
            get { return this.association; }
        }

        public CsdlAssociationSetEnd End1
        {
            get { return this.end1; }
        }

        public CsdlAssociationSetEnd End2
        {
            get { return this.end2; }
        }
    }
}
