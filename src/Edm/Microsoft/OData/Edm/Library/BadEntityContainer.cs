//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM entity container.
    /// </summary>
    internal class BadEntityContainer : BadElement, IEdmEntityContainer
    {
        private readonly string namespaceName;
        private readonly string name;

        public BadEntityContainer(string qualifiedName, IEnumerable<EdmError> errors)
            : base(errors)
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
        }

        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get { return Enumerable.Empty<IEdmEntityContainerElement>(); }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.EntityContainer; }
        }

        public IEdmEntitySet FindEntitySet(string setName)
        {
            return null;
        }

        public IEdmSingleton FindSingleton(string singletonName)
        {
            return null;
        }

        public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
        {
            return null;
        }
    }
}
