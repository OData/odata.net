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
    /// Represents one of the ends of an EDM association set.
    /// </summary>
    internal interface IEdmAssociationSetEnd : IEdmElement
    {
        /// <summary>
        /// Gets the association end that describes the role of this association set end.
        /// </summary>
        IEdmAssociationEnd Role { get; }

        /// <summary>
        /// Gets the entity set this association set end corresponds to.
        /// </summary>
        IEdmEntitySet EntitySet { get; }
    }
}
