//---------------------------------------------------------------------
// <copyright file="CsdlStructuredType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Common base class for CSDL entity and complex Types.
    /// </summary>
    internal abstract class CsdlStructuredType : CsdlElementWithDocumentation
    {
        protected List<CsdlProperty> structuralProperties;
        protected List<CsdlNavigationProperty> navigationProperties;

        protected CsdlStructuredType(IEnumerable<CsdlProperty> structuralProperties, IEnumerable<CsdlNavigationProperty> navigationProperties, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.structuralProperties = new List<CsdlProperty>(structuralProperties);
            this.navigationProperties = new List<CsdlNavigationProperty>(navigationProperties);
        }

        public IEnumerable<CsdlProperty> StructuralProperties
        {
            get { return this.structuralProperties; }
        }

        public IEnumerable<CsdlNavigationProperty> NavigationProperties
        {
            get { return this.navigationProperties; }
        }
    }
}
