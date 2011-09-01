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
using System.Linq;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM structured type definition.
    /// </summary>
    internal abstract class BadStructuredType : BadElement, IEdmStructuredType, IEdmCheckable
    {
        protected BadStructuredType(IEnumerable<EdmError> errors)
            : base(errors)
        {
        }

        public IEdmStructuredType BaseType
        {
            get { return null; }
        }

        public IEnumerable<IEdmProperty> DeclaredProperties
        {
            get { return Enumerable.Empty<IEdmProperty>(); }
        }

        public IEdmProperty FindProperty(string name)
        {
            return null;
        }

        public abstract EdmTypeKind TypeKind
        {
            get;
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsOpen
        {
            get { return false; }
        }
    }
}
