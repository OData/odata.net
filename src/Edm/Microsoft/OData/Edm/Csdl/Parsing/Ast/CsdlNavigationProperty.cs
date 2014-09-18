//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
