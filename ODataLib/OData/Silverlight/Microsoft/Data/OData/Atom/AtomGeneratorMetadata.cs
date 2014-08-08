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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Atom metadata description of a content generator.
    /// </summary>
    public sealed class AtomGeneratorMetadata
    {
        /// <summary>Gets or sets the human readable name of the generator of the content.</summary>
        /// <returns>The human readable name of the generator of the content.</returns>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets or sets the (optional) URI describing the generator of the content.</summary>
        /// <returns>The (optional) URI describing the generator of the content.</returns>
        public Uri Uri
        {
            get;
            set;
        }

        /// <summary>Gets or sets the (optional) version of the generator.</summary>
        /// <returns>The (optional) version of the generator.</returns>
        public string Version
        {
            get;
            set;
        }
    }
}
