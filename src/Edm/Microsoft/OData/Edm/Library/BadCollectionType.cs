//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM collection type.
    /// </summary>
    internal class BadCollectionType : BadType, IEdmCollectionType
    {
        private readonly IEdmTypeReference elementType;

        public BadCollectionType(IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.elementType = new BadTypeReference(new BadType(errors), true);
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Collection; }
        }

        public IEdmTypeReference ElementType
        {
            get { return this.elementType; }
        }
    }
}
