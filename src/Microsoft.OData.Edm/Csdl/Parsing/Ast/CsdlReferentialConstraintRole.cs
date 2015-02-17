//---------------------------------------------------------------------
// <copyright file="CsdlReferentialConstraintRole.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
