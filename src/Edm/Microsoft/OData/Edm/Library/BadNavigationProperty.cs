//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Edm.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
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

        public string DefaultValueString
        {
            get { return null; }
        }

        public EdmConcurrencyMode ConcurrencyMode
        {
            get { return EdmConcurrencyMode.None; }
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
