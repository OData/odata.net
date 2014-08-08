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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Represents a collection of entity reference links (the result of a $links query).
    /// Might include an inline count and a next link.
    /// </summary>
    public sealed class ODataEntityReferenceLinks : ODataAnnotatable
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataMessageWriter"/> for this <see cref="ODataEntityReferenceLinks"/>.
        /// </summary>
        private ODataEntityReferenceLinksSerializationInfo serializationInfo;

        /// <summary>Gets or sets the optional inline count of the $links collection.</summary>
        /// <returns>The optional inline count of the $links collection.</returns>
        public long? Count
        {
            get;
            set;
        }

        /// <summary>Gets or sets the optional next link of the $links collection.</summary>
        /// <returns>The optional next link of the $links collection.</returns>
        public Uri NextPageLink
        {
            get;
            set;
        }

        /// <summary>Gets or sets the enumerable of <see cref="T:Microsoft.Data.OData.ODataEntityReferenceLink" /> instances representing the links of the referenced entities.</summary>
        /// <returns>The enumerable of <see cref="T:Microsoft.Data.OData.ODataEntityReferenceLink" /> instances.</returns>
        /// <remarks>These links should be usable to retrieve or modify the referenced entities.</remarks>
        public IEnumerable<ODataEntityReferenceLink> Links
        {
            get;
            set;
        }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataMessageWriter"/> for this <see cref="ODataEntityReferenceLinks"/>.
        /// </summary>
        internal ODataEntityReferenceLinksSerializationInfo SerializationInfo
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.serializationInfo;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.serializationInfo = ODataEntityReferenceLinksSerializationInfo.Validate(value);
            }
        }
    }
}
