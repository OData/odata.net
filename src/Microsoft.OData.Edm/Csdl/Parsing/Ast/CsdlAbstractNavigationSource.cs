//---------------------------------------------------------------------
// <copyright file="CsdlAbstractNavigationSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an abstract CSDL navigation source.
    /// </summary>
    internal abstract class CsdlAbstractNavigationSource : CsdlNamedElement
    {
        private readonly List<CsdlNavigationPropertyBinding> navigationPropertyBindings;

        public CsdlAbstractNavigationSource(string name, IEnumerable<CsdlNavigationPropertyBinding> navigationPropertyBindings, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.navigationPropertyBindings = new List<CsdlNavigationPropertyBinding>(navigationPropertyBindings);
        }

        public IEnumerable<CsdlNavigationPropertyBinding> NavigationPropertyBindings
        {
            get { return this.navigationPropertyBindings; }
        }
    }
}
