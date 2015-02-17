//---------------------------------------------------------------------
// <copyright file="CsdlReferentialConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL referential constraint.
    /// </summary>
    internal class CsdlReferentialConstraint : CsdlElementWithDocumentation
    {
        private readonly string propertyName;
        private readonly string referencedPropertyName;

        public CsdlReferentialConstraint(string propertyName, string referencedPropertyName, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.propertyName = propertyName;
            this.referencedPropertyName = referencedPropertyName;
        }

        public string PropertyName
        {
            get { return this.propertyName; }
        }

        public string ReferencedPropertyName
        {
            get { return this.referencedPropertyName; }
        }
    }
}
