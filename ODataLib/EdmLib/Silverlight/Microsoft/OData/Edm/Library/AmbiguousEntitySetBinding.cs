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
    internal class AmbiguousEntitySetBinding : AmbiguousBinding<IEdmEntitySet>, IEdmEntitySet
    {
        public AmbiguousEntitySetBinding(IEdmEntitySet first, IEdmEntitySet second)
            : base(first, second)
        {
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        public IEdmEntityContainer Container
        {
            get
            {
                IEdmEntitySet first = this.Bindings.FirstOrDefault();
                return first != null ? first.Container : null;
            }
        }

        public Edm.Expressions.IEdmPathExpression Path
        {
            get { return null; }
        }

        public IEdmType Type
        {
            get { return new EdmCollectionType(new EdmEntityTypeReference(new BadEntityType(String.Empty, this.Errors), false)); }
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
