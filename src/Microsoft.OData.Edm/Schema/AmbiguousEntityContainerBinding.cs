//---------------------------------------------------------------------
// <copyright file="AmbiguousEntityContainerBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
    internal class AmbiguousEntityContainerBinding : AmbiguousBinding<IEdmEntityContainer>, IEdmEntityContainer, IEdmFullNamedElement
    {
        private readonly string namespaceName;
        private readonly string fullName;

        public AmbiguousEntityContainerBinding(IEdmEntityContainer first, IEdmEntityContainer second)
            : base(first, second)
        {
            // Ambiguous entity containers can be produced by either searching for full name or simple name.
            // This results in the reported NamespaceName being ambiguous so the first one is selected arbitrarily.
            this.namespaceName = first.Namespace ?? string.Empty;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.Name);
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.EntityContainer; }
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
