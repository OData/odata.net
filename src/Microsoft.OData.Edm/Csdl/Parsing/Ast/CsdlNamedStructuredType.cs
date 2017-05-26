//---------------------------------------------------------------------
// <copyright file="CsdlNamedStructuredType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Common base class for CSDL entity and complex types.
    /// </summary>
    internal abstract class CsdlNamedStructuredType : CsdlStructuredType
    {
        protected string baseTypeName;
        protected bool isAbstract;
        protected bool isOpen;
        protected string name;

        protected CsdlNamedStructuredType(string name, string baseTypeName, bool isAbstract, bool isOpen, IEnumerable<CsdlProperty> structuralproperties, IEnumerable<CsdlNavigationProperty> navigationProperties, CsdlDocumentation documentation, CsdlLocation location)
            : base(structuralproperties, navigationProperties, documentation, location)
        {
            this.isAbstract = isAbstract;
            this.isOpen = isOpen;
            this.name = name;
            this.baseTypeName = baseTypeName;
        }

        public string BaseTypeName
        {
            get { return this.baseTypeName; }
        }

        public bool IsAbstract
        {
            get { return this.isAbstract; }
        }

        public bool IsOpen
        {
            get { return this.isOpen; }
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
