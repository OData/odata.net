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
using Microsoft.Data.Edm.Library.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Represents a semantically invalid EDM association.
    /// </summary>
    internal class BadAssociation : BadElement, IEdmAssociation
    {
        private string namespaceName;
        private string name;

        public BadAssociation(string qualifiedName, IEnumerable<EdmError> errors)
            : base(errors)
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public IEdmAssociationEnd End1
        {
            get { return new BadAssociationEnd(this, "End1", this.Errors); }
        }

        public IEdmAssociationEnd End2
        {
            get { return new BadAssociationEnd(this, "End2", this.Errors); }
        }

        public CsdlSemanticsReferentialConstraint ReferentialConstraint
        {
            get { return null; }
        }
    }
}
