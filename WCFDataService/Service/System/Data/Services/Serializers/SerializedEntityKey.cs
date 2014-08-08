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

namespace System.Data.Services.Serializers
{
    /// <summary>
    /// Data structure for representing the identity and edit-link of an entity being serialized.
    /// </summary>
    internal abstract class SerializedEntityKey
    {
        /// <summary>
        /// Gets the edit link of the entity relative to the service base.
        /// </summary>
        internal abstract Uri RelativeEditLink { get; }

        /// <summary>
        /// Gets the identity of the entity.
        /// </summary>
        internal abstract string Identity { get; }

        /// <summary>
        /// Gets the absolute edit link of the entity.
        /// </summary>
        internal abstract Uri AbsoluteEditLink { get; }

        /// <summary>
        /// Gets the absolute edit link of the entity without a type segment or other suffix.
        /// </summary>
        internal abstract Uri AbsoluteEditLinkWithoutSuffix { get; }
    }
}
