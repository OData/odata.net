//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
