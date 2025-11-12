//---------------------------------------------------------------------
// <copyright file="BadEntityContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM entity container.
    /// </summary>
    internal class BadEntityContainer : BadElement, IEdmEntityContainer, IEdmFullNamedElement
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly string fullName;

        public BadEntityContainer(string qualifiedName, IEnumerable<EdmError> errors)
            : base(errors)
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name, out this.fullName);
        }

        public IEnumerable<IEdmEntityContainerElement> Elements => Enumerable.Empty<IEdmEntityContainerElement>();

        public string Namespace => this.namespaceName;

        public string Name => this.name;

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName => this.fullName;

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.EntityContainer;

        public IEdmEntitySet FindEntitySet(string name) => null;

        public IEdmEntitySet FindEntitySet(ReadOnlySpan<char> name) => null;

        public IEdmSingleton FindSingleton(string name) => null;

        public IEdmSingleton FindSingleton(ReadOnlySpan<char> name) => null;

        public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName) => null;

        public IEnumerable<IEdmOperationImport> FindOperationImports(ReadOnlySpan<char> operationName) => null;
    }
}
