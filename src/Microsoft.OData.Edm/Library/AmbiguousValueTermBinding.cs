//---------------------------------------------------------------------
// <copyright file="AmbiguousValueTermBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a name binding to more than one item.
    /// </summary>
    internal class AmbiguousValueTermBinding : AmbiguousBinding<IEdmValueTerm>, IEdmValueTerm
    {
        private readonly IEdmValueTerm first;

        // Type cache.
        private readonly Cache<AmbiguousValueTermBinding, IEdmTypeReference> type = new Cache<AmbiguousValueTermBinding, IEdmTypeReference>();
        private static readonly Func<AmbiguousValueTermBinding, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        private readonly string appliesTo = null;
        private readonly string defaultValue = null;

        public AmbiguousValueTermBinding(IEdmValueTerm first, IEdmValueTerm second)
            : base(first, second)
        {
            this.first = first;
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }

        public string Namespace
        {
            get { return this.first.Namespace ?? string.Empty; }
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

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        private IEdmTypeReference ComputeType()
        {
            return new BadTypeReference(new BadType(Errors), true);
        }
    }
}
