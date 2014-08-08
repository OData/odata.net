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
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Represents an entity reference link (the result of a $link query).
    /// </summary>
    [DebuggerDisplay("{Url.OriginalString}")]
    public sealed class ODataEntityReferenceLink : ODataItem
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataMessageWriter"/> for this <see cref="ODataEntityReferenceLink"/>.
        /// </summary>
        private ODataEntityReferenceLinkSerializationInfo serializationInfo;

        /// <summary>Gets or sets the URI representing the URL of the referenced entity.</summary>
        /// <returns>The URI representing the URL of the referenced entity.</returns>
        /// <remarks>This URL should be usable to retrieve or modify the referenced entity.</remarks>
        public Uri Url
        {
            get;
            set;
        }
        
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataMessageWriter"/> for this <see cref="ODataEntityReferenceLink"/>.
        /// </summary>
        internal ODataEntityReferenceLinkSerializationInfo SerializationInfo
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.serializationInfo;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.serializationInfo = ODataEntityReferenceLinkSerializationInfo.Validate(value);
            }
        }
    }
}
