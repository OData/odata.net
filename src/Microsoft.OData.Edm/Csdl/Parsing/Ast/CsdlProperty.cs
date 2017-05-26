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
        private readonly CsdlTypeReference type;
        private readonly string defaultValue;

        public CsdlProperty(string name, CsdlTypeReference type, string defaultValue, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.type = type;
            this.defaultValue = defaultValue;
        }

        public CsdlTypeReference Type
        {
            get { return this.type; }
        }

        public string DefaultValue
        {
            get { return this.defaultValue; }
        }
    }
}
