//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Diagnostics;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a name binding to more than one item.
    /// </summary>
    internal class AmbiguousTypeBinding : AmbiguousBinding<IEdmSchemaType>, IEdmSchemaType
    {
        private readonly string namespaceName;

        public AmbiguousTypeBinding(IEdmSchemaType first, IEdmSchemaType second)
            : base(first, second)
        {
            Debug.Assert(first.Namespace == second.Namespace, "Schema elements should only be ambiguous with other elements in the same namespace");
            this.namespaceName = first.Namespace ?? string.Empty;
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.None; }
        }
    }
}
