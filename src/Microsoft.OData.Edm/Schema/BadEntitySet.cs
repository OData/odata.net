//---------------------------------------------------------------------
// <copyright file="BadEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM entity set.
    /// </summary>
    internal class BadEntitySet : BadElement, IEdmEntitySet
    {
        private readonly string name;
        private readonly IEdmEntityContainer container;

        public BadEntitySet(string name, IEdmEntityContainer container, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.name = name ?? string.Empty;
            this.container = container;
        }

        public string Name
        {
            get { return this.name; }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }

        public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
        {
            get { return Enumerable.Empty<IEdmNavigationPropertyBinding>(); }
        }

        public IEdmPathExpression Path
        {
            get { return null; }
        }

        public IEdmType Type
        {
            get { return new EdmCollectionType(new EdmEntityTypeReference(new BadEntityType(String.Empty, this.Errors), false)); }
        }

        public bool IncludeInServiceDocument
        {
            get { return true; }
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
