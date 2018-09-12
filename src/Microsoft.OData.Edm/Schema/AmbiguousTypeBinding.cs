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
    internal class AmbiguousTypeBinding : AmbiguousBinding<IEdmSchemaType>, IEdmSchemaType, IEdmFullNamedElement
    {
        private readonly string namespaceName;
        private readonly string fullName;

        public AmbiguousTypeBinding(IEdmSchemaType first, IEdmSchemaType second)
            : base(first, second)
        {
            Debug.Assert(first.Namespace == second.Namespace, "Schema elements should only be ambiguous with other elements in the same namespace");
            this.namespaceName = first.Namespace ?? string.Empty;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.Name);
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        public EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.None; }
        }
    }
}
