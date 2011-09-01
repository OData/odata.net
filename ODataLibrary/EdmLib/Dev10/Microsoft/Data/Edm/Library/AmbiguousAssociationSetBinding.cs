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

namespace Microsoft.Data.Edm.Library
{
    internal class AmbiguousAssociationSetBinding : AmbiguousBinding<IEdmAssociationSet>, IEdmAssociationSet
    {
        public AmbiguousAssociationSetBinding(IEdmAssociationSet first, IEdmAssociationSet second)
            : base(first, second)
        {
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.AssociationSet; }
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
    }
}
