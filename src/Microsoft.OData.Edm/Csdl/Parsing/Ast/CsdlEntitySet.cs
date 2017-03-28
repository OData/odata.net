//---------------------------------------------------------------------
// <copyright file="CsdlEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a CSDL Entity Set.
    /// </summary>
    internal class CsdlEntitySet : CsdlAbstractNavigationSource
    {
        private readonly string elementType;

        public CsdlEntitySet(string name, string elementType, IEnumerable<CsdlNavigationPropertyBinding> navigationPropertyBindings, CsdlDocumentation documentation, CsdlLocation location)
            : this(name, elementType, navigationPropertyBindings, documentation, location, true)
        {
        }

        public CsdlEntitySet(string name, string elementType, IEnumerable<CsdlNavigationPropertyBinding> navigationPropertyBindings, CsdlDocumentation documentation, CsdlLocation location, bool includeInServiceDocument)
            : base(name, navigationPropertyBindings, documentation, location)
        {
            this.elementType = elementType;
            this.IncludeInServiceDocument = includeInServiceDocument;
        }

        public string ElementType
        {
            get { return this.elementType; }
        }

        public bool IncludeInServiceDocument { get; private set; }
    }
}
