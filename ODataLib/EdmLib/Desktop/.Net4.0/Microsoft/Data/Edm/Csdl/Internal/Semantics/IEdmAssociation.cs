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

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Represents a definition of an EDM association type.
    /// </summary>
    internal interface IEdmAssociation : IEdmNamedElement
    {
        /// <summary>
        /// Gets the namespace this association belongs to.
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Gets the first end of the association.
        /// </summary>
        IEdmAssociationEnd End1 { get; }

        /// <summary>
        /// Gets the second end of the association.
        /// </summary>
        IEdmAssociationEnd End2 { get; }

        /// <summary>
        /// Gets the referential constraint of the association.
        /// </summary>
        CsdlSemanticsReferentialConstraint ReferentialConstraint { get; }
    }
}
