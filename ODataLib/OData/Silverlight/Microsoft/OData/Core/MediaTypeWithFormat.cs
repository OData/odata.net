//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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
