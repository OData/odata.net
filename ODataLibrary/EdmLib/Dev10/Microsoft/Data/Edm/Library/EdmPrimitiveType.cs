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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a definition of an EDM primitive type.
    /// </summary>
    internal class EdmPrimitiveType : EdmType, IEdmPrimitiveType
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly EdmPrimitiveTypeKind primitiveKind;

        public EdmPrimitiveType(string namespaceName, string name, EdmPrimitiveTypeKind primitiveKind)
            :base(EdmTypeKind.Primitive)
        {
            this.namespaceName = namespaceName ?? string.Empty;
            this.name = name ?? string.Empty;
            this.primitiveKind = primitiveKind;
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public EdmPrimitiveTypeKind PrimitiveKind
        {
            get { return this.primitiveKind; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }
    }
}
