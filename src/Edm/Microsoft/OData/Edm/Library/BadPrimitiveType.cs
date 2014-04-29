//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM primitive type definition.
    /// </summary>
    internal class BadPrimitiveType : BadType, IEdmPrimitiveType
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

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Primitive; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }
    }
}
