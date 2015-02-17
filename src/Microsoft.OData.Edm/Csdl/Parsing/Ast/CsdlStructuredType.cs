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
        protected List<CsdlProperty> properties;

        protected CsdlStructuredType(IEnumerable<CsdlProperty> properties, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.properties = new List<CsdlProperty>(properties);
        }

        public IEnumerable<CsdlProperty> Properties
        {
            get { return this.properties; }
        }
    }
}
