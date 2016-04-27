//---------------------------------------------------------------------
// <copyright file="AmbiguousEntitySetBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
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

        public IEdmPathExpression Path
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
