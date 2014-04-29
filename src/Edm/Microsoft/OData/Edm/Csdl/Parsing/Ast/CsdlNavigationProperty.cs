//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a CSDL navigation property.
    /// </summary>
    internal class CsdlNavigationProperty : CsdlNamedElement
    {
        private readonly string type;
        private readonly bool? nullable;
        private readonly string partner;
        private readonly bool containsTarget;
        private readonly CsdlOnDelete onDelete;
        private readonly IEnumerable<CsdlReferentialConstraint> referentialConstraints;

        public CsdlNavigationProperty(string name, string type, bool? nullable, string partner, bool containsTarget, CsdlOnDelete onDelete, IEnumerable<CsdlReferentialConstraint> referentialConstraints, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.type = type;
            this.nullable = nullable;
            this.partner = partner;
            this.containsTarget = containsTarget;
            this.onDelete = onDelete;
            this.referentialConstraints = referentialConstraints;
        }

        public string Type
        {
            get { return this.type; }
        }

        public bool? Nullable
        {
            get { return this.nullable;  }
        }

        public string Partner
        {
            get { return this.partner; }
        }

        public bool ContainsTarget
        {
            get { return this.containsTarget; }
        }

        public CsdlOnDelete OnDelete
        {
            get { return this.onDelete; }
        }

        public IEnumerable<CsdlReferentialConstraint> ReferentialConstraints
        {
            get { return this.referentialConstraints; }
        }
    }
}
