//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM property.
    /// </summary>
    internal class BadProperty : BadElement, IEdmStructuralProperty
    {
        protected readonly string name;
        protected readonly IEdmStructuredType declaringType;

        // Type cache.
        private readonly Cache<BadProperty, IEdmTypeReference> type = new Cache<BadProperty, IEdmTypeReference>();
        private readonly static Func<BadProperty, IEdmTypeReference> s_computeType = (me) => me.ComputeType();

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
            get { return this.type.GetValue(this, s_computeType, null); }
        }

        private IEdmTypeReference ComputeType()
        {
            // Any bad type reference will do, so using BadRowTypeDefinition because it doesn't require type name.
            return new EdmRowTypeReference(new BadRowType(this.Errors), true);
        }

        public string DefaultValue
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
    }
}
