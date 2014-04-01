//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Internal;
using Microsoft.OData.Edm.Library.Internal;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
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
        private static readonly Func<BadAssociationEnd, BadEntityType> ComputeTypeFunc = (me) => me.ComputeType();

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

        public EdmMultiplicity Multiplicity
        {
            get { return EdmMultiplicity.Unknown; }
        }

        public EdmOnDeleteAction OnDelete
        {
            get { return EdmOnDeleteAction.None; }
        }

        public IEdmEntityType EntityType
        {
            get { return this.type.GetValue(this, ComputeTypeFunc, null); }
        }

        public string Name
        {
            get { return this.role; }
        }

        private BadEntityType ComputeType()
        {
            return new BadEntityType(this.declaringAssociation.Name + "." + this.role, this.Errors);
        }
    }
}
