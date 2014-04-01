//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Library
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

        public IEdmSingleton FindSingleton(string name)
        {
            return null;
        }

        public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
        {
            return null;
        }
    }
}
