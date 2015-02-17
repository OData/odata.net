//---------------------------------------------------------------------
// <copyright file="EdmEntitySetSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;

    internal class EdmEntitySetSimulator : IEdmEntitySet
    {
        public EdmEntitySetSimulator()
        {
            Name = "EntitySetName";
        }

        public string Name { get; set; }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        public IEdmType Type { get; set; }

        public IEdmEntityContainer Container { get; set; }

        public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
        {
            get { return Enumerable.Empty<IEdmNavigationPropertyBinding>(); }
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
        {
            throw new NotImplementedException();
        }

        public Microsoft.OData.Edm.Expressions.IEdmPathExpression Path
        {
            get { return null; }
        }
    }
}