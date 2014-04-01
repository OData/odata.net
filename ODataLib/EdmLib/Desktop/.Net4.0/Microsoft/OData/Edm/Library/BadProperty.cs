//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
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

        public EdmConcurrencyMode ConcurrencyMode
        {
            get { return EdmConcurrencyMode.None; }
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
