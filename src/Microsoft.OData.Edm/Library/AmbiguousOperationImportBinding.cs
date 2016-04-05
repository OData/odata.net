//---------------------------------------------------------------------
// <copyright file="AmbiguousOperationImportBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Library
{
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Class that represents an unresolved operation import binding to two or more operation imports.
    /// </summary>
    internal class AmbiguousOperationImportBinding : AmbiguousBinding<IEdmOperationImport>, IEdmOperationImport
    {
        private readonly IEdmOperationImport first;

        public AmbiguousOperationImportBinding(IEdmOperationImport first, IEdmOperationImport second)
            : base(first, second)
        {
            this.first = first;
        }

        public IEdmOperation Operation
        {
            get { return this.first.Operation; }
        }

        public IEdmTypeReference ReturnType
        {
            get { return null; }
        }

        public IEdmEntityContainer Container
        {
            get { return first.Container; }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return first.ContainerElementKind; }
        }

        public IEdmExpression EntitySet
        {
            get { return null; }
        }
    }
}
