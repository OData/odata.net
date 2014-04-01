//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
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

        public Edm.Expressions.IEdmPathExpression Path
        {
            get { return null; }
        }

        public IEdmType Type
        {
            get { return new EdmCollectionType(new EdmEntityTypeReference(new BadEntityType(String.Empty, this.Errors), false)); }
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty property)
        {
            return null;
        }
    }
}
