//---------------------------------------------------------------------
// <copyright file="CsdlSingleton.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a CSDL Singleton.
    /// </summary>
    internal class CsdlSingleton : CsdlAbstractNavigationSource
    {
        private readonly string type;

        public CsdlSingleton(string name, string type, IEnumerable<CsdlNavigationPropertyBinding> navigationPropertyBindings, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, navigationPropertyBindings, documentation, location)
        {
            this.type = type;
        }

        public string Type
        {
            get { return this.type; }
        }
    }
}
