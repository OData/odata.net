//---------------------------------------------------------------------
// <copyright file="BadProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM property.
    /// </summary>
    internal class BadProperty : BadElement, IEdmStructuralProperty
    {
        private readonly string name;
        private readonly IEdmStructuredType declaringType;

        // Type cache.
        private readonly Cache<BadProperty, IEdmTypeReference> type = new Cache<BadProperty, IEdmTypeReference>();
        private static readonly Func<BadProperty, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public BadProperty(IEdmStructuredType declaringType, string name, IEnumerable<EdmError> errors)
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

        public string DefaultValueString
        {
            get { return null; }
        }

        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.None; }
        }

        public override string ToString()
        {
            EdmError error = this.Errors.FirstOrDefault();
            Debug.Assert(error != null, "error != null");
            return error.ErrorCode + ":" + this.ToTraceString();
        }

        private IEdmTypeReference ComputeType()
        {
            return new BadTypeReference(new BadType(Errors), true);
        }
    }
}
