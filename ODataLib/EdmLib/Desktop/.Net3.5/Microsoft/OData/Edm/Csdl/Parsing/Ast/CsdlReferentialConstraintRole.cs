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
    /// Represents a CSDL referential constraint role.
    /// </summary>
    internal class CsdlReferentialConstraintRole : CsdlElementWithDocumentation
    {
        private readonly string role;
        private readonly List<CsdlPropertyReference> properties;

        public CsdlReferentialConstraintRole(string role, IEnumerable<CsdlPropertyReference> properties, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.role = role;
            this.properties = new List<CsdlPropertyReference>(properties);
        }

        public string Role
        {
            get { return this.role; }
        }

        public IEnumerable<CsdlPropertyReference> Properties
        {
            get { return this.properties; }
        }

        public int IndexOf(CsdlPropertyReference reference)
        {
            return this.properties.IndexOf(reference);
        }
    }
}
