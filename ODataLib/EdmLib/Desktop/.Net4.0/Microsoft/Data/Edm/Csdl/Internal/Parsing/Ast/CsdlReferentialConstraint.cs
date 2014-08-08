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
    /// Represents a CSDL referential constraint.
    /// </summary>
    internal class CsdlReferentialConstraint : CsdlElementWithDocumentation
    {
        private readonly CsdlReferentialConstraintRole principal;
        private readonly CsdlReferentialConstraintRole dependent;

        public CsdlReferentialConstraint(CsdlReferentialConstraintRole principal, CsdlReferentialConstraintRole dependent, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.principal = principal;
            this.dependent = dependent;
        }

        public CsdlReferentialConstraintRole Principal
        {
            get { return this.principal; }
        }

        public CsdlReferentialConstraintRole Dependent
        {
            get { return this.dependent; }
        }
    }
}
