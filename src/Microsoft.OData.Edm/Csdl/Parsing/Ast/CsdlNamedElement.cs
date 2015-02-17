//---------------------------------------------------------------------
// <copyright file="CsdlNamedElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Common base class for all named CSDL elements.
    /// </summary>
    internal abstract class CsdlNamedElement : CsdlElementWithDocumentation
    {
        private readonly string name;

        protected CsdlNamedElement(string name, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
