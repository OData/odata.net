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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// A helper class to associate a <see cref="ODataFormat"/> with a media type.
    /// </summary>
    internal sealed class MediaTypeWithFormat
    {
        /// <summary>The media type.</summary>
        public MediaType MediaType
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="ODataFormat"/> for this media type.
        /// </summary>
        public ODataFormat Format
        {
            get;
            set;
        }
    }
}
