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
        private readonly string baseTermName;

        public CsdlTerm(string name, CsdlTypeReference type, string baseTermName, string appliesTo, string defaultValue, CsdlLocation location)
            : base(name, location)
        {
            this.type = type;
            this.baseTermName = baseTermName;
            this.appliesTo = appliesTo;
            this.defaultValue = defaultValue;
        }

        public CsdlTypeReference Type
        {
            get { return this.type; }
        }

        public string BaseTermName
        {
            get { return this.baseTermName; }
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
