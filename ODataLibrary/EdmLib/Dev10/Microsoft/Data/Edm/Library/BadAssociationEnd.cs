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
    /// Represents a semantically invalid EDM association end.
    /// </summary>
    internal class BadAssociationEnd : BadElement, IEdmAssociationEnd
    {
        private readonly string role;
        private readonly IEdmAssociation declaringAssociation;

        // Type cache.
        private readonly Cache<BadAssociationEnd, BadEntityType> type = new Cache<BadAssociationEnd, BadEntityType>();
        private readonly static Func<BadAssociationEnd, BadEntityType> s_computeType = (me) => me.ComputeType();

        public BadAssociationEnd(IEdmAssociation declaringAssociation, string role, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.role = role ?? string.Empty;
            this.declaringAssociation = declaringAssociation;
        }

        public IEdmAssociation DeclaringAssociation
        {
            get { return this.declaringAssociation; }
        }

        public EdmAssociationMultiplicity Multiplicity
        {
            get { return EdmAssociationMultiplicity.Unknown; }
        }

        public EdmOnDeleteAction OnDelete
        {
            get { return EdmOnDeleteAction.None; }
        }

        public IEdmEntityType EntityType
        {
            get { return this.type.GetValue(this, s_computeType, null); }
        }

        public string Name
        {
            get { return this.role; }
        }

        private BadEntityType ComputeType()
        {
            return new BadEntityType(this.role, this.Errors);
        }
    }
}
