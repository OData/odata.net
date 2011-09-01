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
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library
{
    /// <summary> 
    /// Represents a semantically invalid EDM association set.
    /// </summary>
    internal class BadAssociationSet : BadElement, IEdmAssociationSet
    {
        private readonly string name;

        public BadAssociationSet(string name, IEnumerable<EdmError> errors)
            :base(errors)
        {
            this.name = name ?? string.Empty;
        }

        public IEdmAssociation Association
        {
            get { return new BadAssociation(String.Empty, this.Errors); }
        }

        public IEdmAssociationSetEnd End1
        {
            get { return new BadAssociationSetEnd(this, "End1", this.Errors); }
        }

        public IEdmAssociationSetEnd End2
        {
            get { return new BadAssociationSetEnd(this, "End2", this.Errors); }
        }

        public string Name
        {
            get { return this.name; }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.AssociationSet; }
        }
    }
}
