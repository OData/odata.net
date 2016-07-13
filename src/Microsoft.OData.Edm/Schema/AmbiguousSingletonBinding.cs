//---------------------------------------------------------------------
// <copyright file="AmbiguousSingletonBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
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

        public IEdmPathExpression Path
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

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
        {
            return null;
        }

        public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
        {
            return null;
        }
    }
}
