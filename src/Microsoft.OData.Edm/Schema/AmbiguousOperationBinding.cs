//---------------------------------------------------------------------
// <copyright file="AmbiguousOperationBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    internal class AmbiguousOperationBinding : AmbiguousBinding<IEdmOperation>, IEdmOperation, IEdmFullNamedElement
    {
        private readonly string fullName;
        private IEdmOperation first;

        public AmbiguousOperationBinding(IEdmOperation first, IEdmOperation second)
            : base(first, second)
        {
            this.first = first;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Namespace, this.Name);
        }

        public IEdmTypeReference ReturnType
        {
            // Not using the typical behavior for first.ReturnType as returning null is the old behavior.
            get { return null; }
        }

        public string Namespace
        {
            get { return this.first.Namespace; }
        }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.first.Parameters; }
        }

        public bool IsBound
        {
            get { return this.first.IsBound; }
        }

        public IEdmPathExpression EntitySetPath
        {
            get { return this.first.EntitySetPath; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return this.first.SchemaElementKind; }
        }

        public IEdmOperationParameter FindParameter(string name)
        {
            return this.first.FindParameter(name);
        }
    }
}
