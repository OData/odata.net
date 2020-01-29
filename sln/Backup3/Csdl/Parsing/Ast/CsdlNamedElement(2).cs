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
    internal abstract class CsdlNamedElement : CsdlElement
    {
        private readonly string name;

        protected CsdlNamedElement(string name, CsdlLocation location)
            : base(location)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
