//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
