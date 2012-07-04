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
    /// Represents an EDM association set.
    /// </summary>
    internal interface IEdmAssociationSet : IEdmNamedElement
    {
        /// <summary>
        /// Gets the association of this association set.
        /// </summary>
        IEdmAssociation Association { get; }

        /// <summary>
        /// Gets the first end of this association set.
        /// </summary>
        IEdmAssociationSetEnd End1 { get; }

        /// <summary>
        /// Gets the second end of this association set.
        /// </summary>
        IEdmAssociationSetEnd End2 { get; }

        IEdmEntityContainer Container { get; }
    }
}
