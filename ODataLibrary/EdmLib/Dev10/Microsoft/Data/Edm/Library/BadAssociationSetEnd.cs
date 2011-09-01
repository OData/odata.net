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
    /// Represents a semantically invalid EDM association end.
    /// </summary>
    internal class BadAssociationSetEnd : BadElement, IEdmAssociationSetEnd
    {
        private readonly string role;
        private readonly IEdmAssociationSet associationSet;

        public BadAssociationSetEnd(IEdmAssociationSet declaringAssociationSet, string role, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.role = role;
            this.associationSet = declaringAssociationSet;
        }

        public IEdmAssociationEnd Role
        {
            get { return new BadAssociationEnd(this.associationSet.Association, this.role, this.Errors); }
        }

        public IEdmEntitySet EntitySet
        {
            get { return new BadEntitySet(this.role, this.Errors); }
        }
    }
}
