//---------------------------------------------------------------------
// <copyright file="BadNavigationProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm.Validation;

    /// <summary>
    /// Represents a semantically invalid EDM navigation property.
    /// </summary>
    internal class BadNavigationProperty : BadElement, IEdmNavigationProperty
    {
        private readonly string name;
        private readonly IEdmStructuredType declaringType;

        // Type cache.
        private readonly Cache<BadNavigationProperty, IEdmTypeReference> type = new Cache<BadNavigationProperty, IEdmTypeReference>();
        private static readonly Func<BadNavigationProperty, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public BadNavigationProperty(IEdmStructuredType declaringType, string name, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.name = name ?? string.Empty;
            this.declaringType = declaringType;
        }

        public string Name
        {
            get { return this.name; }
        }

        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
        }

        public IEdmTypeReference Type
        {
            get { return this.type.GetValue(this, ComputeTypeFunc, null); }
        }

        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.None; }
        }

        public IEdmNavigationProperty Partner
        {
            get { return null; }
        }

        public EdmOnDeleteAction OnDelete
        {
            get { return EdmOnDeleteAction.None; }
        }

        public IEdmReferentialConstraint ReferentialConstraint
        {
            get { return null; }
        }

        public bool ContainsTarget
        {
            get { return false; }
        }

        public override string ToString()
        {
            EdmError error = this.Errors.FirstOrDefault();
            Debug.Assert(error != null, "error != null");
            return error.ErrorCode + ":" + this.ToTraceString();
        }

        private IEdmTypeReference ComputeType()
        {
            return new BadTypeReference(new BadType(this.Errors), true);
        }
    }
}