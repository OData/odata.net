//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Data.Edm.Library.Internal
{
    internal class AmbiguousEntitySetBinding : AmbiguousBinding<IEdmEntitySet>, IEdmEntitySet
    {
        public AmbiguousEntitySetBinding(IEdmEntitySet first, IEdmEntitySet second)
            : base(first, second)
        {
        }

        public IEdmEntityType ElementType
        {
            get { return new BadEntityType(String.Empty, this.Errors); }
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

        public IEnumerable<IEdmNavigationTargetMapping> NavigationTargets
        {
            get { return Enumerable.Empty<IEdmNavigationTargetMapping>(); }
        }

        public IEdmEntitySet FindNavigationTarget(IEdmNavigationProperty property)
        {
            return null;
        }
    }
}
