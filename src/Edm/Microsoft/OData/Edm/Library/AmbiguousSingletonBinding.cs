//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Library
{
    internal class AmbiguousSingletonBinding : AmbiguousBinding<IEdmSingleton>, IEdmSingleton
    {
        public AmbiguousSingletonBinding(IEdmSingleton first, IEdmSingleton second)
            : base(first, second)
        {
        }

        public IEdmType Type
        {
            get { return new BadEntityType(String.Empty, this.Errors); }
        }


        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.Singleton; }
        }

        public IEdmEntityContainer Container
        {
            get
            {
                IEdmSingleton first = this.Bindings.FirstOrDefault();
                return first != null ? first.Container : null;
            }
        }

        public Edm.Expressions.IEdmPathExpression Path
        {
            get { return null; }
        }

        public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
        {
            get { return Enumerable.Empty<IEdmNavigationPropertyBinding>(); }
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty property)
        {
            return null;
        }
    }
}
