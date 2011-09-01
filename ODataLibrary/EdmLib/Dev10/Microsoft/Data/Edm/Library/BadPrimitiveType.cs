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

using System.Collections.Generic;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM primitive type definition.
    /// </summary>
    internal class BadPrimitiveType : BadElement, IEdmPrimitiveType
    {
        private readonly EdmPrimitiveTypeKind primitiveKind;
        private readonly string name;
        private readonly string namespaceName;

        public BadPrimitiveType(string qualifiedName, EdmPrimitiveTypeKind primitiveKind, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.primitiveKind = primitiveKind;
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
        }

        public EdmPrimitiveTypeKind PrimitiveKind
        {
            get { return this.primitiveKind; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Primitive; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }
    }
}
