//---------------------------------------------------------------------
// <copyright file="AmbiguousTermBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a name binding to more than one item.
    /// </summary>
    internal class AmbiguousTermBinding : AmbiguousBinding<IEdmTerm>, IEdmTerm, IEdmFullNamedElement
    {
        private readonly IEdmTerm first;
        private readonly string fullName;

        // Type cache.
        private readonly Cache<AmbiguousTermBinding, IEdmTypeReference> type = new Cache<AmbiguousTermBinding, IEdmTypeReference>();
        private static readonly Func<AmbiguousTermBinding, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        private readonly string appliesTo = null;
        private readonly string defaultValue = null;

        public AmbiguousTermBinding(IEdmTerm first, IEdmTerm second)
            : base(first, second)
        {
            this.first = first;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Namespace, this.Name);
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Term; }
        }

        public string Namespace
        {
            get { return this.first.Namespace ?? string.Empty; }
        }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        public IEdmTypeReference Type
        {
            get { return this.type.GetValue(this, ComputeTypeFunc, null); }
        }

        public string AppliesTo
        {
            get { return this.appliesTo; }
        }

        public string DefaultValue
        {
            get { return this.defaultValue; }
        }

        private IEdmTypeReference ComputeType()
        {
            return new BadTypeReference(new BadType(Errors), true);
        }
    }
}
