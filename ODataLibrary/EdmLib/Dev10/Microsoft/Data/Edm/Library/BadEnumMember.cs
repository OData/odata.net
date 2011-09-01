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

using System.Collections.Generic;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM enumeration type member.
    /// </summary>
    internal class BadEnumMember : BadElement, IEdmEnumMember
    {
        private readonly string name;
        private readonly IEdmEnumType declaringType;
        private readonly IEdmPrimitiveValue value;

        public BadEnumMember(IEdmEnumType declaringType, string name, IEdmPrimitiveValue value, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.name = name ?? string.Empty;
            this.declaringType = declaringType;
            this.value = value;
        }

        public string Name
        {
            get { return this.name; }
        }

        public IEdmEnumType DeclaringType
        {
            get { return this.declaringType; }
        }

        public IEdmPrimitiveValue Value
        {
            get { return this.value; }
        }
    }
}
