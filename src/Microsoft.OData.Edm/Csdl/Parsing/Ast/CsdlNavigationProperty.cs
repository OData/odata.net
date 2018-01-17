//---------------------------------------------------------------------
// <copyright file="CsdlNavigationProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
        private readonly IEdmPathExpression partnerPath;
        private readonly bool containsTarget;
        private readonly CsdlOnDelete onDelete;
        private readonly IEnumerable<CsdlReferentialConstraint> referentialConstraints;

        public CsdlNavigationProperty(string name, string type, bool? nullable, string partner, bool containsTarget, CsdlOnDelete onDelete, IEnumerable<CsdlReferentialConstraint> referentialConstraints, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.type = type;
            this.nullable = nullable;
            this.partnerPath = partner == null ? null : new EdmPathExpression(partner);
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

        public IEdmPathExpression PartnerPath
        {
            get { return this.partnerPath; }
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
