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

namespace System.Data.OData.Atom
{
    /// <summary>
    /// Enumeration which is used to store different multiValue state for a given property mapping.
    /// </summary>
    internal enum EntityPropertyMappingMultiValueStatus
    {
        /// <summary>
        /// The info represents property mapping which has nothing to do with multiValues.
        /// </summary>
        None,

        /// <summary>
        /// The info represents property mapping for the multiValue property itself.
        /// </summary>
        MultiValueProperty,

        /// <summary>
        /// The info represents property mapping for a subproperty of a multiValue property (either the item itself, or one of the properties on the item).
        /// </summary>
        MultiValueItemProperty
    }
}
