//---------------------------------------------------------------------
// <copyright file="CsdlComplexType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL complex type.
    /// </summary>
    internal class CsdlComplexType : CsdlNamedStructuredType
    {
        public CsdlComplexType(string name, string baseTypeName, bool isAbstract, bool isOpen, IEnumerable<CsdlProperty> structuralProperties, IEnumerable<CsdlNavigationProperty> navigationProperties, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, baseTypeName, isAbstract, isOpen, structuralProperties, navigationProperties, documentation, location)
        {
        }
    }
}
