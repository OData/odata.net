//---------------------------------------------------------------------
// <copyright file="CsdlEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL entity type.
    /// </summary>
    internal class CsdlEntityType : CsdlNamedStructuredType
    {
        private readonly CsdlKey key;
        private readonly bool hasStream;

        public CsdlEntityType(string name, string baseTypeName, bool isAbstract, bool isOpen, bool hasStream, CsdlKey key, IEnumerable<CsdlProperty> structualProperties, IEnumerable<CsdlNavigationProperty> navigationProperties, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, baseTypeName, isAbstract, isOpen, structualProperties, navigationProperties, documentation, location)
        {
            this.key = key;
            this.hasStream = hasStream;
        }

        public CsdlKey Key
        {
            get { return this.key; }
        }

        public bool HasStream
        {
            get { return this.hasStream; }
        }
    }
}
