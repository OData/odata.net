//---------------------------------------------------------------------
// <copyright file="CsdlTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL term.
    /// </summary>
    internal class CsdlTerm : CsdlNamedElement
    {
        private readonly CsdlTypeReference type;
        private readonly string appliesTo;
        private readonly string defaultValue;

        public CsdlTerm(string name, CsdlTypeReference type, string appliesTo, string defaultValue, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.type = type;
            this.appliesTo = appliesTo;
            this.defaultValue = defaultValue;
        }

        public CsdlTypeReference Type
        {
            get { return this.type; }
        }

        public string AppliesTo
        {
            get { return this.appliesTo; }
        }

        public string DefaultValue
        {
            get { return this.defaultValue; }
        }
    }
}
