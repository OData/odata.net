//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast
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
