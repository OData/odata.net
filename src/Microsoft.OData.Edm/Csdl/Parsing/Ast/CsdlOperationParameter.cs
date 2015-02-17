//---------------------------------------------------------------------
// <copyright file="CsdlOperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL operation parameter.
    /// </summary>
    internal class CsdlOperationParameter : CsdlNamedElement
    {
        private readonly CsdlTypeReference type;

        public CsdlOperationParameter(string name, CsdlTypeReference type, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.type = type;
        }

        public CsdlTypeReference Type
        {
            get { return this.type; }
        }
    }
}
