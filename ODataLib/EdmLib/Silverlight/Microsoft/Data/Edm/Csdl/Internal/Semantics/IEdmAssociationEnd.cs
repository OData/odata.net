//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    /// Represents an end of an EDM association.
    /// </summary>
    internal interface IEdmAssociationEnd : IEdmNamedElement
    {
        /// <summary>
        /// Gets the declaring association of this association end.
        /// </summary>
        IEdmAssociation DeclaringAssociation { get; }

        /// <summary>
        /// Gets the entity type of this end of the association.
        /// </summary>
        IEdmEntityType EntityType { get; }

        /// <summary>
        /// Gets this end's multiplicity.
        /// </summary>
        EdmMultiplicity Multiplicity { get; }
        
        /// <summary>
        /// Gets the action to execute on the deletion of this association end.
        /// </summary>
        EdmOnDeleteAction OnDelete { get; }
    }
}
