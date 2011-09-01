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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    #endregion

    /// <summary>
    /// Represents a media resource.
    /// </summary>
    public sealed class ODataStreamReferenceValue : ODataAnnotatable
    {
        /// <summary>
        /// Edit link for media resource.
        /// </summary>
        public Uri EditLink
        {
            get;
            set;
        }

        /// <summary>
        /// Read link for media resource.
        /// </summary>
        public Uri ReadLink
        {
            get;
            set;
        }

        /// <summary>
        /// Content media type.
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Media resource ETag.
        /// </summary>
        public string ETag
        {
            get;
            set;
        }
    }
}
