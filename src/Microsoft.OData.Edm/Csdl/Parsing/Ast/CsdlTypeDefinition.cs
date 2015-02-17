//---------------------------------------------------------------------
// <copyright file="CsdlTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL type definition.
    /// </summary>
    internal class CsdlTypeDefinition : CsdlNamedElement
    {
        private readonly string underlyingTypeName;

        public CsdlTypeDefinition(string name, string underlyingTypeName, CsdlLocation location)
            : base(name, /*documentation*/null, location)
        {
            this.underlyingTypeName = underlyingTypeName;
        }

        public string UnderlyingTypeName
        {
            get { return this.underlyingTypeName; }
        }
    }
}
