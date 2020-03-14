//---------------------------------------------------------------------
// <copyright file="CsdlProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL property.
    /// </summary>
    internal class CsdlProperty : CsdlNamedElement
    {
        public CsdlProperty() { }

        public CsdlProperty(string name, CsdlTypeReference type, string defaultValue, CsdlLocation location)
            : base(name, location)
        {
            Type = type;
            DefaultValue = defaultValue;
        }

        public CsdlTypeReference Type { get; set; }

        public string DefaultValue { get; set; }
    }
}
