//---------------------------------------------------------------------
// <copyright file="AmbiguousTypeBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;

namespace Microsoft.OData.Edm
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
