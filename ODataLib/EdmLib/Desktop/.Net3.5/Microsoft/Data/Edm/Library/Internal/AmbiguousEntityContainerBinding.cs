//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Data.Edm.Library.Internal
{
    internal class AmbiguousEntityContainerBinding : AmbiguousBinding<IEdmEntityContainer>, IEdmEntityContainer
    {
        private readonly string namespaceName;

        public AmbiguousEntityContainerBinding(IEdmEntityContainer first, IEdmEntityContainer second)
            : base(first, second)
        {
            // Ambiguous entity containers can be produced by either searching for full name or simple name.
            // This results in the reported NamespaceName being ambiguous so the first one is selected arbitrarily.
            this.namespaceName = first.Namespace ?? string.Empty;
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.EntityContainer; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get { return Enumerable.Empty<IEdmEntityContainerElement>(); }
        }

        public IEdmEntitySet FindEntitySet(string name)
        {
            return null;
        }

        public IEnumerable<IEdmFunctionImport> FindFunctionImports(string name)
        {
            return null;
        }
    }
}
